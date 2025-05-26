#ifndef MQTT_CLIENT_H
#define MQTT_CLIENT_H

#include <PubSubClient.h>
#include <WiFiClientSecure.h>
#include "WiFi/CustomWiFiManager.h"
#include "JsonSerializer.h"

class MqttManager {
public:
    MqttManager(CustomWiFiManager& customWiFiManager,
            PubSubClient& mqttManager,
            WiFiClientSecure& wifiClient,
            const char* server,
            int port,
            const char* username,
            const char* password,
            const char* deviceId,
            const char* dataTopic,
            const char* statusTopic,
            JsonSerializer& jsonSerializer); 
    
    bool setup();
    void loop();
    bool publishSensorData(float temperature, float humidity, float gas, float particles);
    bool clearRetainedMessage(const char* topic);
    
private:
    CustomWiFiManager& _customWiFiManager;
    PubSubClient& _mqttManager;
    WiFiClientSecure& _wifiClient;
    
    const char* _server;
    int _port;
    const char* _username;
    const char* _password;
    const char* _deviceId;
    const char* _dataTopic;
    const char* _statusTopic;
    
    JsonSerializer& _jsonSerializer;

    String _clientId;
    unsigned long _lastStatusUpdate;
    String _connectionTime;
    
    bool connect();
    String createStatusJson(bool isConnected);
    String createSensorJson(float temperature, float humidity, float gas, float particles);
};

#endif