#ifndef CUSTOM_WIFI_MANAGER_H
#define CUSTOM_WIFI_MANAGER_H

#include "WiFiConnection.h"
#include "ConfigPortalManager.h"

/**
 * @brief Manages WiFi connection and configuration portal for the device.
 */
class CustomWiFiManager {
private:
    WiFiConnection _WiFiconnection;   // Handles WiFi connection logic.
    WiFiManager _wifiManager;         // WiFiManager instance for portal management.
    ConfigPortalManager _portalManager; // Handles captive portal setup and logic.
    int _configButtonPin;             // GPIO pin for WiFi config button.
    
public:
    /**
     * @brief Constructs a CustomWiFiManager with WiFi credentials, AP info, and config button pin.
     * @param ssid WiFi network SSID.
     * @param password WiFi network password.
     * @param apName Access Point name for config portal.
     * @param apPassword Access Point password for config portal.
     * @param configButtonPin GPIO pin for config button.
     */
    CustomWiFiManager(const char* ssid, const char* password, 
                     const char* apName, const char* apPassword,
                     int configButtonPin);

    /**
     * @brief Attempts to connect to WiFi or starts config portal if needed.
     * @return true if connected, false otherwise.
     */
    bool connect();

    /**
     * @brief Checks if the device is currently connected to WiFi.
     * @return true if connected, false otherwise.
     */
    bool isConnected();

    /**
     * @brief Disconnects from the current WiFi network.
     */
    void disconnect();

    /**
     * @brief Resets WiFi settings and restarts the device.
     */
    void resetSettings();

    /**
     * @brief Starts the WiFi configuration portal in non-blocking mode.
     */
    void startConfigPortal();

    /**
     * @brief Checks the config button state and triggers a reset if pressed.
     */
    void checkConfigButton();

    /**
     * @brief Main loop for the WiFi manager. Checks the config button state.
     */
    void loop();

    /**
     * @brief Handles LCD updates and portal logic while the config portal is active.
     */
    void handleConfigPortal();
};

#endif // CUSTOM_WIFI_MANAGER_H