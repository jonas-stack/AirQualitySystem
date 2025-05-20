#ifndef CONFIG_H
#define CONFIG_H

// MQTT settings
#define MQTT_SERVER " "
#define MQTT_PORT 8883
#define MQTT_USERNAME " "
#define MQTT_PASSWORD " "

// Device identification
#define DEVICE_ID " "

// WiFi configuration
#define WIFI_SSID "YourSSID"        // Default SSID, will be overridden by stored settings
#define WIFI_PASSWORD "YourPassword" // Default password, will be overridden by stored settings

// WiFi configuration AP name
#define WIFI_AP_NAME " "
#define WIFI_AP_PASSWORD " " 

// Double Reset Detector settings
#define DRD_TIMEOUT 10 
#define DRD_ADDRESS 0   

#endif // CONFIG_H