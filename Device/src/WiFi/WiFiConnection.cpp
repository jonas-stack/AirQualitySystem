#include "WiFi/WiFiConnection.h"
#include "devices/lcd_display.h"

/**
 * Constructor for WiFiConnection.
 * Stores SSID and password for later use.
 */
WiFiConnection::WiFiConnection(const char* ssid, const char* password) :
    _ssid(ssid),
    _password(password) {
}

/**
 * Connects to WiFi using the provided credentials.
 * Displays connection progress and handles timeout.
 * @return true if connected successfully, false otherwise.
 */
bool WiFiConnection::connect() {
    displayMessage("WiFi", "Connecting...");
    Serial.println("Connecting to WiFi with provided credentials...");

    WiFi.disconnect(true);  // Ensure any previous connection is closed.
    delay(1000);
    WiFi.mode(WIFI_STA);    // Set WiFi to station mode.
    WiFi.begin(_ssid, _password);

    displayMessage("Connecting to", _ssid);
    unsigned long startTime = millis();
    int dots = 0;

    // Wait for connection or timeout.
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");

        // Update LCD with connection progress.
        if (++dots % 10 == 0) {
            String progressMsg = "Connecting";
            for (int i = 0; i < (dots % 4); i++) {
                progressMsg += ".";
            }
            displayMessage("WiFi", progressMsg.c_str());
        }

        // Abort if connection takes too long.
        if (millis() - startTime > 30000) {
            Serial.println("\nFailed to connect to WiFi");
            displayMessage("WiFi Connection", "Failed!");
            return false;
        }
    }

    Serial.println("\nWiFi connected!");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());

    displayMessage("WiFi Connected", "Successfully!");
    delay(2000);

    String ipString = WiFi.localIP().toString();
    displayMessage("IP:", ipString.c_str());

    return true;
}

/**
 * Checks if the device is currently connected to WiFi.
 * @return true if connected, false otherwise.
 */
bool WiFiConnection::isConnected() {
    return WiFi.status() == WL_CONNECTED;
}

/**
 * Disconnects from the current WiFi network.
 */
void WiFiConnection::disconnect() {
    WiFi.disconnect();
}

/**
 * Returns the current IP address as a string.
 */
String WiFiConnection::getIp() {
    return WiFi.localIP().toString();
}

/**
 * Returns the SSID of the currently connected network.
 */
String WiFiConnection::getSsid() {
    return WiFi.SSID();
}