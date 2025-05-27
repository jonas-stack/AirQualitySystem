#include "WiFi/CustomWiFiManager.h"
#include "MQTT/config.h"
#include "devices/lcd_display.h"

// Debounce time for the WiFi config button (milliseconds), which prevents multiple triggers from a single press.
constexpr int WIFI_CONFIG_BUTTON_DEBOUNCE = 50;

/**
 * Constructs a CustomWiFiManager with WiFi credentials, AP info, and config button pin.
 */
CustomWiFiManager::CustomWiFiManager(const char* ssid, const char* password, 
                                     const char* apName, const char* apPassword,
                                     int configButtonPin)
    : _WiFiconnection(ssid, password),
      _portalManager(_wifiManager, apName, apPassword),
      _configButtonPin(configButtonPin)
{
    if (_configButtonPin >= 0) {
        pinMode(_configButtonPin, INPUT_PULLUP);
        Serial.print("WiFi config button initialized on GPIO ");
        Serial.println(_configButtonPin);
    }
}

/**
 * Attempts to connect using saved WiFi credentials or starts AP portal if needed.
 * Updates the LCD with connection status.
 * @return true if connected, false otherwise.
 */
bool CustomWiFiManager::connect() {
    displayMessage("WiFi", "Connecting...");
    if (_portalManager.autoConnect()) {
        displayMessage("WiFi Connected", "Using saved");
        delay(1000);
        displayMessage("IP:", _WiFiconnection.getIp().c_str());
        delay(1000);
        return true;
    }
    return _WiFiconnection.connect();
}

/**
 * Checks if the device is connected to WiFi as a station and has a valid SSID.
 * @return true if connected, false otherwise.
 */
bool CustomWiFiManager::isConnected() {
    return WiFi.status() == WL_CONNECTED && WiFi.SSID().length() > 0;
}

/**
 * Resets WiFi settings and restarts the device.
 */
void CustomWiFiManager::resetSettings() {
    _portalManager.resetSettings();
    ESP.restart();
}

/**
 * Starts the WiFi configuration portal in non-blocking mode.
 */
void CustomWiFiManager::startConfigPortal() {
    WiFi.disconnect(true); 
    delay(100);
    _portalManager.setConfigPortalBlocking(false);
    _portalManager.startPortal();
}

/**
 * Checks the config button state and triggers a reset if pressed.
 * Debounces the button input.
 */
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

/**
 * Updates the LCD with portal information every 2 seconds in a non-blocking way.
 * Cycles through AP name, password, and portal IP.
 * Should be called frequently while the config portal is active.
 */
void CustomWiFiManager::handleConfigPortal() {
    static unsigned long lastLcdUpdate = 0;
    static int lcdStep = 0;
    unsigned long currentMillis = millis();

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

    _portalManager.process();
}

/**
 * Main loop for the WiFi manager. Checks the config button state.
 */
void CustomWiFiManager::loop() {
    checkConfigButton();
}