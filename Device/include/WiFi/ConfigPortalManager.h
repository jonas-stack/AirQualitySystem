#ifndef CONFIG_PORTAL_MANAGER_H
#define CONFIG_PORTAL_MANAGER_H

#include <WiFiManager.h>

/**
 * @brief Handles the WiFi configuration portal logic using WiFiManager.
 */
class ConfigPortalManager {
private:
    WiFiManager& _wifiManager;      // Reference to the WiFiManager instance.
    const char* _apName;            // Access Point name for the config portal.
    const char* _apPassword;        // Access Point password for the config portal.
    
public:
    /**
     * @brief Constructs a ConfigPortalManager with WiFiManager reference and AP credentials.
     * @param wifiManager Reference to a WiFiManager instance.
     * @param apName Access Point name for the config portal.
     * @param apPassword Access Point password for the config portal.
     */
    ConfigPortalManager(WiFiManager& wifiManager, const char* apName, const char* apPassword);

    /**
     * @brief Gets the Access Point name for the config portal.
     * @return AP name string.
     */
    const char* getApName() const { return _apName; }

    /**
     * @brief Gets the Access Point password for the config portal.
     * @return AP password string.
     */
    const char* getApPassword() const { return _apPassword; }

    /**
     * @brief Starts the WiFi configuration portal in AP mode.
     * @return true if the portal started successfully, false otherwise.
     */
    bool startPortal();

    /**
     * @brief Attempts to connect using saved WiFi credentials.
     * @return true if connection is successful, false otherwise.
     */
    bool autoConnect();

    /**
     * @brief Resets all WiFi settings stored by the WiFiManager.
     */
    void resetSettings();

    /**
     * @brief Sets whether the config portal should block execution or run non-blocking.
     * @param blocking True for blocking mode, false for non-blocking.
     */
    void setConfigPortalBlocking(bool blocking);

    /**
     * @brief Processes background tasks for the WiFiManager.
     *        Should be called frequently in the main loop when the portal is active.
     */
    void process();
};

#endif // CONFIG_PORTAL_MANAGER_H