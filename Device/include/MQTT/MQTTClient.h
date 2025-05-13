#pragma once

#include <Arduino.h>
#include "WiFiManager.h"
#include "TimeManager.h" 
#include "ConnectionManager.h"
#include "DeviceStatusManager.h"
#include "MQTT/config.h"

class MQTTClient {
private:
    // Component managers instead of direct member variables
    ConnectionManager* _connectionManager;
    DeviceStatusManager* _deviceStatusManager;
    const char* _deviceId;
    
    // Create the component managers
    void setupComponents(
        WiFiManager* wifiManager, 
        TimeManager* timeManager, 
        const char* server, 
        int port, 
        const char* username, 
        const char* password);
        
public:
    MQTTClient(
        WiFiManager* wifiManager, 
        TimeManager* timeManager, 
        const char* server, 
        int port, 
        const char* username, 
        const char* password,
        const char* deviceId);
        
    ~MQTTClient();
    
    // Core functionality
    bool setup();
    void loop();
    bool isConnected();
    
    // Message related
    bool publish(const char* topic, const char* payload, bool retain = false);
    bool publishSensorData(float temperature, float humidity, float gas, float particles);
    bool clearRetainedMessage(const char* topic);
};