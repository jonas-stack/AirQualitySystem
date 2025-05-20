#ifndef CONFIG_PORTAL_MANAGER_H
#define CONFIG_PORTAL_MANAGER_H

#include <WiFiManager.h>

class ConfigPortalManager {
private:
    WiFiManager _wifiManager;
    const char* _apName;
    const char* _apPassword;
    
public:
    ConfigPortalManager(const char* apName, const char* apPassword);
    
    bool startPortal();
    bool autoConnect();
    void resetSettings();
    void setConfigTimeout(int seconds);
    void setConnectTimeout(int seconds);
};

#endif // CONFIG_PORTAL_MANAGER_H