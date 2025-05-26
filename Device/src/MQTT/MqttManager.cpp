#include "MQTT/MqttManager.h"
#include "JsonSerializer.h"
#include "WiFi/CustomWiFiManager.h"

MqttManager::MqttManager(
    CustomWiFiManager& customWiFiManager,
    PubSubClient& mqttManager,
    WiFiClientSecure& wifiClient,
    const char* server,
    int port,
    const char* username,
    const char* password,
    const char* deviceId,
    const char* dataTopic,
    const char* statusTopic,
    JsonSerializer& jsonSerializer
) :
    _customWiFiManager(customWiFiManager),
    _mqttManager(mqttManager),
    _wifiClient(wifiClient),
    _server(server),
    _port(port),
    _username(username),
    _password(password),
    _deviceId(deviceId),
    _dataTopic(dataTopic),
    _statusTopic(statusTopic),
    _jsonSerializer(jsonSerializer),
    _lastStatusUpdate(0)
{
    _clientId = "ESP32Client-";
    _clientId += String(random(0xffff), HEX);
}

bool MqttManager::setup() {
    _wifiClient.setInsecure();
    _mqttManager.setServer(_server, _port);
    _mqttManager.setBufferSize(1024);
    return connect();
}

bool MqttManager::connect() {
    if (!_customWiFiManager.isConnected()) {
        return false;
    }

    // Use JsonSerializer for offline message (LWT)
    String offlineMsg = _jsonSerializer.serializeStatusMessage("offline", _deviceId);

    bool connected = _mqttManager.connect(
        _clientId.c_str(), 
        _username, 
        _password,
        _statusTopic,
        1,
        true,
        offlineMsg.c_str()
    );

    if (connected) {
        _connectionTime = time(nullptr);
        String onlineMsg = _jsonSerializer.serializeStatusMessage("online", _deviceId);
        _mqttManager.publish(_statusTopic, onlineMsg.c_str(), true);
    }

    return connected;
}

void MqttManager::loop() {
    if (!_mqttManager.connected()) {
        connect();
    }
    
    _mqttManager.loop();
    
    // Update status every 5 minutes
    unsigned long now = millis();
    if (now - _lastStatusUpdate > 300000) {
        _lastStatusUpdate = now;
        if (_mqttManager.connected()) {
            String statusMsg = createStatusJson(true);
            _mqttManager.publish(_statusTopic, statusMsg.c_str(), true);
        }
    }
}

String MqttManager::createStatusJson(bool isConnected) {
    // Use JsonSerializer for status message
    return _jsonSerializer.serializeStatusMessage(isConnected ? "online" : "offline", _deviceId);
}

String MqttManager::createSensorJson(float temperature, float humidity, float gas, float particles) {
    // Use JsonSerializer for sensor data
    return _jsonSerializer.serializeSensorData(temperature, humidity, gas, particles, _deviceId);
}

bool MqttManager::publishSensorData(float temperature, float humidity, float gas, float particles) {
    if (!_mqttManager.connected()) {
        if (!connect()) {
            return false;
        }
    }
    
    String jsonString = createSensorJson(temperature, humidity, gas, particles);
    return _mqttManager.publish(_dataTopic, jsonString.c_str(), false);
}

bool MqttManager::clearRetainedMessage(const char* topic) {
    if (!_mqttManager.connected()) {
        if (!connect()) {
            return false;
        }
    }
    return _mqttManager.publish(topic, "", true);
}