#pragma once

#include <WiFiClientSecure.h>
#include <PubSubClient.h>
#include <Arduino.h>
#include "WiFiManager.h"
#include "TimeManager.h"

// Callback signature from PubSubClient
typedef void (*MessageCallback)(char*, byte*, unsigned int);

class ConnectionManager {
private:
    WiFiManager* _wifiManager;
    const char* _server;
    int _port;
    const char* _username;
    const char* _password;
    String _clientId;
    
    WiFiClientSecure _espClient;
    PubSubClient* _client = nullptr;
    bool _configured = false;
    
public:
    ConnectionManager(WiFiManager* wifiManager, 
                      const char* server, 
                      int port,
                      const char* username,
                      const char* password);
    ~ConnectionManager();
    
    bool setup();
    void loop();
    bool isConnected();
    bool connect(const char* willTopic = nullptr, 
                 int willQos = 0, 
                 bool willRetain = false, 
                 const char* willMessage = nullptr);
    void disconnect();
    
    // Publish wrapper method
    bool publish(const char* topic, const char* payload, bool retain = false);
    
    // Getters
    PubSubClient* getClient() { return _client; }
    String getClientId() const { return _clientId; }
};