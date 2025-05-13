#include "WiFiManager.h"

WiFiManager::WiFiManager(const char* ssid, const char* password) : _ssid(ssid), _password(password) {
}

bool WiFiManager::connect() {
    delay(10);
    Serial.println();
    Serial.print("Connecting to ");
    Serial.println(_ssid);

    WiFi.mode(WIFI_STA);
    WiFi.begin(_ssid, _password);

    unsigned long startTime = millis();
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
        
        // Timeout after 30 seconds
        if (millis() - startTime > 30000) {
            Serial.println("\nFailed to connect to WiFi");
            return false;
        }
    }

    randomSeed(micros());

    Serial.println("");
    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
    
    return true;
}

bool WiFiManager::isConnected() {
    return WiFi.status() == WL_CONNECTED;
}

void WiFiManager::disconnect() {
    WiFi.disconnect();
    Serial.println("WiFi disconnected");
}