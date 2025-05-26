#ifndef CUSTOM_WIFI_MANAGER_H
#define CUSTOM_WIFI_MANAGER_H

#include "WiFiConnection.h"
#include "ConfigPortalManager.h"

class CustomWiFiManager {
private:
    WiFiConnection _connection;  
    WiFiManager _wifiManager;
    ConfigPortalManager _portalManager;
    int _configButtonPin;
    
public:
    CustomWiFiManager(const char* ssid, const char* password, 
                     const char* apName, const char* apPassword,
                     int configButtonPin);
    
    bool connect();
    bool isConnected();
    void disconnect();
    void resetSettings();
    void startConfigPortal();
    void checkConfigButton();
    void loop();
    void handleConfigPortal();
};

#endif // CUSTOM_WIFI_MANAGER_H