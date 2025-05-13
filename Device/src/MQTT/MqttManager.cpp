#include "MqttManager.h"
#include <ArduinoJson.h>

MQTTClient::MQTTClient(
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

MQTTClient::~MQTTClient() {
    if (_mqttClient) {
        delete _mqttClient;
    }
}

bool MQTTClient::setup() {
    _wifiClient.setInsecure();
    _mqttClient = new PubSubClient(_wifiClient);
    _mqttClient->setServer(_server, _port);
    _mqttClient->setBufferSize(1024);
    
    return connect();
}

bool MQTTClient::connect() {
    if (!_wifiManager->isConnected()) {
        return false;
    }
    
    String offlineMsg = createStatusJson(false);
    
    bool connected = _mqttClient->connect(
        _clientId.c_str(), 
        _username, 
        _password,
        _statusTopic,
        1,
        true,
        offlineMsg.c_str()
    );
    
    if (connected) {
        _connectionTime = _timeManager->getCurrentTime();
        String onlineMsg = createStatusJson(true);
        _mqttClient->publish(_statusTopic, onlineMsg.c_str(), true);
    }
    
    return connected;
}

void MQTTClient::loop() {
    if (!_mqttClient->connected()) {
        connect();
    }
    
    _mqttClient->loop();
    
    // Update status every 5 minutes
    unsigned long now = millis();
    if (now - _lastStatusUpdate > 300000) {
        _lastStatusUpdate = now;
        if (_mqttClient->connected()) {
            String statusMsg = createStatusJson(true);
            _mqttClient->publish(_statusTopic, statusMsg.c_str(), true);
        }
    }
}

String MQTTClient::createStatusJson(bool isConnected) {
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = _deviceId;
    doc["IsConnected"] = isConnected;
    doc["LastSeen"] = _timeManager->getCurrentTime();
    
    if (isConnected) {
        if (_connectionTime.isEmpty()) {
            _connectionTime = _timeManager->getCurrentTime();
        }
        doc["ConnectedSince"] = _connectionTime;
    } else if (!_connectionTime.isEmpty()) {
        doc["ConnectedSince"] = _connectionTime;
    }
    
    String message;
    serializeJson(doc, message);
    return message;
}

String MQTTClient::createSensorJson(float temperature, float humidity, float gas, float particles) {
    DynamicJsonDocument doc(256);
    
    doc["temperature"] = temperature;
    doc["humidity"] = humidity;
    doc["air_quality"] = gas;
    doc["pm25"] = particles;
    doc["device_id"] = _deviceId;
    doc["timestamp"] = _timeManager->getCurrentTime();
    
    String output;
    serializeJson(doc, output);
    return output;
}

bool MQTTClient::publishSensorData(float temperature, float humidity, float gas, float particles) {
    if (!_mqttClient->connected()) {
        if (!connect()) {
            return false;
        }
    }
    
    String jsonString = createSensorJson(temperature, humidity, gas, particles);
    return _mqttClient->publish(_dataTopic, jsonString.c_str(), false);
}

bool MQTTClient::clearRetainedMessage(const char* topic) {
    if (!_mqttClient->connected()) {
        if (!connect()) {
            return false;
        }
    }
    return _mqttClient->publish(topic, "", true);
}