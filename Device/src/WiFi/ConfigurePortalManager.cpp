#include "WiFi/ConfigPortalManager.h"
#include "devices/lcd_display.h"

/**
 * Constructs a ConfigPortalManager with a reference to the WiFiManager and AP credentials.
 */
ConfigPortalManager::ConfigPortalManager(WiFiManager& wifiManager, const char* apName, const char* apPassword)
    : _wifiManager(wifiManager),
      _apName(apName),
      _apPassword(apPassword) {
}

/**
 * Starts the WiFi configuration portal in AP mode.
 * Sets a timeout and prints AP credentials to serial.
 * @return true if the portal started successfully, false otherwise.
 */
bool ConfigPortalManager::startPortal() {
    _wifiManager.setConfigPortalTimeout(180); // Set portal timeout to 3 minutes.
    WiFi.mode(WIFI_AP_STA); // Enable both AP and STA modes.

    Serial.println("Starting config portal with:");
    Serial.print("SSID: ");
    Serial.println(_apName);
    Serial.print("Password: ");
    Serial.println(_apPassword);

    return _wifiManager.startConfigPortal(_apName, _apPassword);
}

/**
 * Attempts to connect using saved WiFi credentials.
 * @return true if connection is successful, false otherwise.
 */
bool ConfigPortalManager::autoConnect() {
    Serial.println("Trying autoConnect with saved credentials...");
    return _wifiManager.autoConnect(_apName, _apPassword);
}

/**
 * Resets all WiFi settings stored by the WiFiManager.
 */
void ConfigPortalManager::resetSettings() {
    Serial.println("Resetting WiFi settings");
    _wifiManager.resetSettings();
}

/**
 * Sets whether the config portal should block execution or run non-blocking.
 */
void ConfigPortalManager::setConfigPortalBlocking(bool blocking) {
    _wifiManager.setConfigPortalBlocking(blocking);
}

/**
 * Processes background tasks for the WiFiManager.
 * Should be called frequently in the main loop when the portal is active.
 */
void ConfigPortalManager::process() {
    _wifiManager.process();
}