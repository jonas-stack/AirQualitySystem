#include "WiFi/WiFiConnection.h"
#include "devices/lcd_display.h"

WiFiConnection::WiFiConnection(const char* ssid, const char* password) :
    _ssid(ssid),
    _password(password) {
}

bool WiFiConnection::connect() {
    displayMessage("WiFi", "Connecting...");
    Serial.println("Connecting to WiFi with provided credentials...");
    
    WiFi.disconnect(true);  // Disconnect from any previous connections
    delay(1000);            // Give it time to disconnect
    WiFi.mode(WIFI_STA);    // Set to station mode
    WiFi.begin(_ssid, _password);

    // Show connection progress
    displayMessage("Connecting to", _ssid);
    unsigned long startTime = millis();
    int dots = 0;
    
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
        
        // Display connection progress with dots
        if (++dots % 10 == 0) {
            String progressMsg = "Connecting";
            for (int i = 0; i < (dots % 4); i++) {
                progressMsg += ".";
            }
            displayMessage("WiFi", progressMsg.c_str());
        }
        
        // Check for timeout
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

bool WiFiConnection::isConnected() {
    return WiFi.status() == WL_CONNECTED;
}

void WiFiConnection::disconnect() {
    WiFi.disconnect();
}

String WiFiConnection::getIp() {
    return WiFi.localIP().toString();
}

String WiFiConnection::getSsid() {
    return WiFi.SSID();
}

int WiFiConnection::getSignalStrength() {
    return WiFi.RSSI();
}