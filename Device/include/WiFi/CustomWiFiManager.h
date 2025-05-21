#ifndef CUSTOM_WIFI_MANAGER_H
#define CUSTOM_WIFI_MANAGER_H

#include "WiFiConnection.h"
#include "ConfigPortalManager.h"

class CustomWiFiManager {
private:
    WiFiConnection _connection;
    ConfigPortalManager _portalManager;
    bool _configMode;
    int _configButtonPin;
    
public:
    CustomWiFiManager(const char* ssid, const char* password, 
                    const char* apName, const char* apPassword,
                    int configButtonPin = -1);
    
    bool connect();
    bool isConnected();
    void disconnect();
    void resetSettings();
    void startConfigPortal();
    void checkConfigButton();
    void loop();
};

#endif // CUSTOM_WIFI_MANAGER_H