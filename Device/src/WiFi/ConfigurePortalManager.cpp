#include "WiFi/ConfigPortalManager.h"
#include "devices/lcd_display.h"

ConfigPortalManager::ConfigPortalManager(WiFiManager& _wifiManager, const char* apName, const char* apPassword) :
    _wifiManager(_wifiManager),
    _apName(apName),
    _apPassword(apPassword) {
}

bool ConfigPortalManager::startPortal() {
    // Configure WiFi manager settings
    _wifiManager.setConfigPortalTimeout(180); // 3 minutes timeout
    
    // Set AP mode to ensure visibility
    WiFi.mode(WIFI_AP_STA);
    
    // Start config portal
    Serial.println("Starting config portal with:");
    Serial.print("SSID: ");
    Serial.println(_apName);
    Serial.print("Password: ");
    Serial.println(_apPassword);
    
    return _wifiManager.startConfigPortal(_apName, _apPassword);
}

bool ConfigPortalManager::autoConnect() {
    Serial.println("Trying autoConnect with saved credentials...");
    return _wifiManager.autoConnect(_apName, _apPassword);
}

void ConfigPortalManager::resetSettings() {
    Serial.println("Resetting WiFi settings");
    _wifiManager.resetSettings();
}

void ConfigPortalManager::setConfigTimeout(int seconds) {
    _wifiManager.setConfigPortalTimeout(seconds);
}

void ConfigPortalManager::setConnectTimeout(int seconds) {
    _wifiManager.setConnectTimeout(seconds);
}