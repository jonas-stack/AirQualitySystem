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
    // Only return true if connected as STA and has a valid SSID
    return WiFi.status() == WL_CONNECTED && WiFi.SSID().length() > 0;
}

void CustomWiFiManager::resetSettings() {
    _portalManager.resetSettings();
    ESP.restart();
}

void CustomWiFiManager::startConfigPortal() {
    WiFi.disconnect(true); 
    delay(100);
    // Start portal in non-blocking mode
    _portalManager.setConfigPortalBlocking(false);
    _portalManager.startPortal(); // Don't check the return value here!
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

// update the LCD display with a message in a non-blocking way repeating every 2 seconds
void CustomWiFiManager::handleConfigPortal() {
    static unsigned long lastLcdUpdate = 0;
    static int lcdStep = 0;
    unsigned long currentMillis = millis();

    // Update LCD every 2 seconds with portal info, non-blocking
    if (currentMillis - lastLcdUpdate > 2000) {
        switch (lcdStep) {
            case 0:
                displayMessage("WiFi Setup", ("AP: " + String(_portalManager.getApName())).c_str());
                break;
            case 1:
                displayMessage("Password:", _portalManager.getApPassword());
                break;
            case 2:
                displayMessage("Go to:", "192.168.4.1");
                break;
        }
        lcdStep = (lcdStep + 1) % 3;
        lastLcdUpdate = currentMillis;
    }

    // Call process frequently!
    _portalManager.process();
}

void CustomWiFiManager::loop() {
    checkConfigButton();
}