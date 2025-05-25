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
    
    const char* getApName() const { return _apName; }
    const char* getApPassword() const { return _apPassword; }
    bool startPortal();
    bool autoConnect();
    void resetSettings();
    void setConfigPortalBlocking(bool blocking);
    void process();
};

#endif // CONFIG_PORTAL_MANAGER_H