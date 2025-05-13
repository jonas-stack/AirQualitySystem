#include "WiFiManager.h"

WiFiManager::WiFiManager(const char* ssid, const char* password) : _ssid(ssid), _password(password) {}

bool WiFiManager::connect() {
    WiFi.mode(WIFI_STA);
    WiFi.begin(_ssid, _password);

    unsigned long startTime = millis();
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        if (millis() - startTime > 30000) {
            return false;
        }
    }
    randomSeed(micros());
    return true;
}

bool WiFiManager::isConnected() {
    return WiFi.status() == WL_CONNECTED;
}

void WiFiManager::disconnect() {
    WiFi.disconnect();
}