#include "MqttManager.h"
#include <ArduinoJson.h>

MqttManager::MqttManager(
    WiFiManager* wifiManager, 
    TimeManager* timeManager, 
    const char* server, 
    int port, 
    const char* username, 
    const char* password,
    const char* deviceId,
    const char* dataTopic,
    const char* statusTopic
) : 
    _wifiManager(wifiManager),
    _timeManager(timeManager),
    _server(server),
    _port(port),
    _username(username),
    _password(password),
    _deviceId(deviceId),
    _dataTopic(dataTopic),
    _statusTopic(statusTopic),
    _lastStatusUpdate(0) {
    
    _clientId = "ESP32Client-";
    _clientId += String(random(0xffff), HEX);
}

MqttManager::~MqttManager() {
    if (_mqttManager) {
        delete _mqttManager;
    }
}

bool MqttManager::setup() {
    _wifiClient.setInsecure();
    _mqttManager = new PubSubClient(_wifiClient);
    _mqttManager->setServer(_server, _port);
    _mqttManager->setBufferSize(1024);
    
    return connect();
}

bool MqttManager::connect() {
    if (!_wifiManager->isConnected()) {
        return false;
    }
    
    // Don't set LastSeen in the offline message
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = _deviceId;
    doc["IsConnected"] = false;
    doc["LastSeen"] = _timeManager->getUnixTime();
    
    String offlineMsg;
    serializeJson(doc, offlineMsg);
    
    // Here's where the LWT is configured - these parameters set up the LWT:
    // LWT acts as a failsafe to infor other client connected to broker when a device disconnects
    bool connected = _mqttManager->connect(
        _clientId.c_str(), 
        _username, 
        _password,
        _statusTopic,
        1,
        true,
        offlineMsg.c_str()
    );
    
    if (connected) {
        _connectionTime = _timeManager->getUnixTime();
        String onlineMsg = createStatusJson(true);
        _mqttManager->publish(_statusTopic, onlineMsg.c_str(), true);
    }
    
    return connected;
}

void MqttManager::loop() {
    if (!_mqttManager->connected()) {
        connect();
    }
    
    _mqttManager->loop();
    
    // Update status every 5 minutes
    unsigned long now = millis();
    if (now - _lastStatusUpdate > 300000) {
        _lastStatusUpdate = now;
        if (_mqttManager->connected()) {
            String statusMsg = createStatusJson(true);
            _mqttManager->publish(_statusTopic, statusMsg.c_str(), true);
        }
    }
}

String MqttManager::createStatusJson(bool isConnected) {
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = _deviceId;
    doc["IsConnected"] = isConnected;
    doc["LastSeen"] = _timeManager->getUnixTime();
    
    String message;
    serializeJson(doc, message);
    return message;
}

String MqttManager::createSensorJson(float temperature, float humidity, float gas, float particles) {
    DynamicJsonDocument doc(256);
    
    doc["temperature"] = temperature;
    doc["humidity"] = humidity;
    doc["air_quality"] = gas;
    doc["pm25"] = particles;
    doc["device_id"] = _deviceId;
    doc["timestamp"] = _timeManager->getUnixTime();
    
    String output;
    serializeJson(doc, output);
    return output;
}

bool MqttManager::publishSensorData(float temperature, float humidity, float gas, float particles) {
    if (!_mqttManager->connected()) {
        if (!connect()) {
            return false;
        }
    }
    
    String jsonString = createSensorJson(temperature, humidity, gas, particles);
    return _mqttManager->publish(_dataTopic, jsonString.c_str(), false);
}

bool MqttManager::clearRetainedMessage(const char* topic) {
    if (!_mqttManager->connected()) {
        if (!connect()) {
            return false;
        }
    }
    return _mqttManager->publish(topic, "", true);
}