#ifndef CONFIG_H
#define CONFIG_H

// MQTT settings (replace with your broker details)
#define MQTT_SERVER " "         // MQTT broker address
#define MQTT_PORT 8883          // MQTT broker port (default for TLS)
#define MQTT_USERNAME " "       // MQTT username
#define MQTT_PASSWORD " "       // MQTT password

// Device identification
#define DEVICE_ID " "           // Unique device identifier

// WiFi configuration (default values, can be overridden by stored settings)
#define WIFI_SSID "YourSSID"        // Default WiFi SSID
#define WIFI_PASSWORD "YourPassword" // Default WiFi password

// WiFi configuration AP (Access Point) name and password for config portal
#define WIFI_AP_NAME " "            // AP name for configuration portal
#define WIFI_AP_PASSWORD " "        // AP password for configuration portal

#endif // CONFIG_H