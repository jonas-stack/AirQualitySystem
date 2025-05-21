#ifndef CUSTOM_WIFI_MANAGER_H
#define CUSTOM_WIFI_MANAGER_H

#include "WiFiConnection.h"
#include "ConfigPortalManager.h"

class CustomWiFiManager {
private:
    WiFiConnection* _connection;
    ConfigPortalManager* _portalManager;
    bool _configMode;
    
    // Config button handling
    int _configButtonPin;
    bool _lastButtonState;
    unsigned long _lastDebounceTime;
    
    void handleButtonPress();
    void blinkLED(int times, int delayMs);
    
public:
    CustomWiFiManager(const char* ssid, const char* password, 
                    const char* apName, const char* apPassword,
                    int configButtonPin = -1);
    ~CustomWiFiManager();
    
    bool connect();
    bool isConnected();
    void disconnect();
    void resetSettings();
    void startConfigPortal();
    void checkConfigButton();
    void loop();
};

#endif // CUSTOM_WIFI_MANAGER_H