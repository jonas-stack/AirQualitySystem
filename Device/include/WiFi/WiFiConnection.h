#ifndef WIFI_CONNECTION_H
#define WIFI_CONNECTION_H

#include <WiFi.h>

/**
 * @brief Handles WiFi connection logic for the device.
 */
class WiFiConnection {
private:
    const char* _ssid;      // WiFi network SSID.
    const char* _password;  // WiFi network password.
    
public:
    /**
     * @brief Constructs a WiFiConnection with the given credentials.
     * @param ssid WiFi network SSID.
     * @param password WiFi network password.
     */
    WiFiConnection(const char* ssid, const char* password);

    /**
     * @brief Connects to the configured WiFi network.
     * @return true if connected successfully, false otherwise.
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
     * @brief Gets the current IP address as a string.
     * @return IP address.
     */
    String getIp();

    /**
     * @brief Gets the SSID of the currently connected network.
     * @return SSID string.
     */
    String getSsid();

    /**
     * @brief Gets the current WiFi signal strength (RSSI).
     * @return Signal strength in dBm.
     */
    int getSignalStrength();
};

#endif // WIFI_CONNECTION_H