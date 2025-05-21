#include "WiFi/CustomWiFiManager.h"
#include "MQTT/config.h"
#include "devices/lcd_display.h"

constexpr int WIFI_CONFIG_BUTTON_DEBOUNCE = 50;

CustomWiFiManager::CustomWiFiManager(const char* ssid, const char* password, 
                                     const char* apName, const char* apPassword,
                                     int configButtonPin)
    : _connection(ssid, password),
      _portalManager(apName, apPassword),
      _configButtonPin(configButtonPin)
{
    if (_configButtonPin >= 0) {
        pinMode(_configButtonPin, INPUT_PULLUP);
        Serial.print("WiFi config button initialized on GPIO ");
        Serial.println(_configButtonPin);
    }
}

bool CustomWiFiManager::connect() {
    displayMessage("WiFi", "Connecting...");
    if (_portalManager.autoConnect()) {
        displayMessage("WiFi Connected", "Using saved");
        delay(1000);
        displayMessage("IP:", _connection.getIp().c_str());
        delay(1000);
        return true;
    }
    return _connection.connect();
}

bool CustomWiFiManager::isConnected() {
    return _connection.isConnected();
}

void CustomWiFiManager::disconnect() {
    _connection.disconnect();
}

void CustomWiFiManager::resetSettings() {
    _portalManager.resetSettings();
    ESP.restart();
}

void CustomWiFiManager::startConfigPortal() {
    Serial.println("Starting config portal");
    displayMessage("WiFi Setup Mode", "Connect to AP:");
    delay(1000);
    displayMessage(WIFI_AP_NAME, WIFI_AP_PASSWORD);
    delay(2000);
    displayMessage("Enter in URL", "http://192.168.4.1");
    delay(3000);

    if (!_portalManager.startPortal()) {
        Serial.println("Config portal failed");
        displayMessage("WiFi Setup", "Failed/Timeout");
        delay(2000);
        ESP.restart();
    } else {
        Serial.println("WiFi connected via portal");
        displayMessage("WiFi Connected", "Via Portal");
        delay(1000);
        displayMessage("IP:", _connection.getIp().c_str());
        delay(1000);
    }
}

void CustomWiFiManager::checkConfigButton() {
    if (_configButtonPin < 0) return;

    bool buttonState = digitalRead(_configButtonPin) == LOW; // Active LOW
    static bool lastButtonState = HIGH;
    static unsigned long lastDebounceTime = 0;

    if (buttonState != lastButtonState) {
        lastDebounceTime = millis();
    }

    if ((millis() - lastDebounceTime) > WIFI_CONFIG_BUTTON_DEBOUNCE) {
        if (buttonState == LOW && lastButtonState == HIGH) {
            resetSettings();
        }
    }
    lastButtonState = buttonState;
}

void CustomWiFiManager::loop() {
    checkConfigButton();
}