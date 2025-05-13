#include "MQTTClient.h"
#include "MQTT/config.h"
#include "JsonSerializer.h"

// Constructor
MQTTClient::MQTTClient(
    WiFiManager* wifiManager, 
    TimeManager* timeManager, 
    const char* server, 
    int port, 
    const char* username, 
    const char* password,
    const char* deviceId
) : _deviceId(deviceId) {
    
    setupComponents(wifiManager, timeManager, server, port, username, password);
}

// Setup component managers
void MQTTClient::setupComponents(
    WiFiManager* wifiManager, 
    TimeManager* timeManager, 
    const char* server, 
    int port, 
    const char* username, 
    const char* password) {
    
    // Create connection manager
    _connectionManager = new ConnectionManager(
        wifiManager,
        server,
        port,
        username,
        password
    );
    
    // Create device status manager
    _deviceStatusManager = new DeviceStatusManager(
        _connectionManager,
        timeManager,
        _deviceId,
        "airquality/status",
        5 * 60 * 1000  // 5 minutes update interval
    );
}

// Destructor
MQTTClient::~MQTTClient() {
    if (_deviceStatusManager) {
        delete _deviceStatusManager;
    }
    if (_connectionManager) {
        delete _connectionManager;
    }
}

// Setup connection
bool MQTTClient::setup() {
    pinMode(LED_BUILTIN, OUTPUT); // Initialize the LED_BUILTIN pin as an output
    
   
    if (!_connectionManager->setup()) {
        return false;
    }
    

    String offlineMessage = _deviceStatusManager->prepareOfflineMessage();
    
    
    if (!_connectionManager->connect(
            "airquality/status",  // LWT topic
            1,                    // QoS 1
            true,                 // Retain
            offlineMessage.c_str())) {
        return false;
    }
    
    _deviceStatusManager->publishOnlineStatus();
    
    return _connectionManager->isConnected();
}

// Check connection
bool MQTTClient::isConnected() {
    return _connectionManager->isConnected();
}

// Maintenance loop
void MQTTClient::loop() {
    
    if (!_connectionManager->isConnected()) {

        _deviceStatusManager->resetConnectionTime();
        
        String offlineMessage = _deviceStatusManager->prepareOfflineMessage();
        
        if (_connectionManager->connect(
                "airquality/status",
                1,
                true,
                offlineMessage.c_str())) {

            _deviceStatusManager->updateConnectionTime();
            _deviceStatusManager->publishOnlineStatus();
        }
    }
    
    _connectionManager->loop();
    _deviceStatusManager->checkStatusUpdate();
}

// Generic publish method
bool MQTTClient::publish(const char* topic, const char* payload, bool retain) {
    return _connectionManager->publish(topic, payload, retain);
}

// Publish sensor data
bool MQTTClient::publishSensorData(float temperature, float humidity, float gas, float particles) {
    if (!_connectionManager->isConnected()) {
        // Handle reconnect via the loop method before publishing
        loop();
        
        if (!_connectionManager->isConnected()) {
            Serial.println("Cannot publish sensor data: not connected to MQTT broker");
            return false;
        }
    }
    
    String jsonString = JsonSerializer::serializeSensorData(temperature, humidity, gas, particles, _deviceId);
    bool success = _connectionManager->publish(MQTT_TOPIC, jsonString.c_str(), false);
    
    if (success) {
        Serial.println("Sensor data published to MQTT");
    } else {
        Serial.println("Failed to publish sensor data");
    }
    
    return success;
}

// Clear retained messages
bool MQTTClient::clearRetainedMessage(const char* topic) {
    return _connectionManager->publish(topic, "", true);
}