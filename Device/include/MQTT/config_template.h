#ifndef CONFIG_H
#define CONFIG_H

// WiFi settings
#define WIFI_SSID "wifi_name_here"
#define WIFI_PASSWORD "wifi_password_here"

// MQTT settings
#define MQTT_SERVER "mqtt_broker_url_here"
#define MQTT_PORT 8883
#define MQTT_USERNAME "mqtt_username_here" 
#define MQTT_PASSWORD "mqtt_password_here"
#define MQTT_TOPIC "airquality/data"

// Device identification
#define DEVICE_ID "AirQuality-ESP32"

// Root CA Certificate
#define ROOT_CA_CERTIFICATE \
"-----BEGIN CERTIFICATE-----\n" \
"... insert your CA certificate here ...\n" \
"-----END CERTIFICATE-----\n"

#endif // CONFIG_H