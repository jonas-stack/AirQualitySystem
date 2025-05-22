#ifndef WIFI_CONNECTION_H
#define WIFI_CONNECTION_H

#include <WiFi.h>

class WiFiConnection {
private:
    const char* _ssid;
    const char* _password;
    
public:
    WiFiConnection(const char* ssid, const char* password);
    
    bool connect();
    bool isConnected();
    void disconnect();
    String getIp();
    String getSsid();
    int getSignalStrength();
};

#endif // WIFI_CONNECTION_H