#pragma once

#include <WiFi.h>
#include <Arduino.h>

class WiFiManager {
private:
    const char* _ssid;
    const char* _password;
    
public:
    WiFiManager(const char* ssid, const char* password);
    bool connect();
    bool isConnected();
    void disconnect();
};

