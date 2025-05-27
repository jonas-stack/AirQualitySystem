#ifndef MQTT_CLIENT_H
#define MQTT_CLIENT_H

#include <PubSubClient.h>
#include <WiFiClientSecure.h>
#include "WiFi/CustomWiFiManager.h"
#include "JsonSerializer.h"

/**
 * @brief Handles MQTT connection, publishing, and status reporting for the device.
 */
class MqttManager {
public:
    /**
     * @brief Constructs an MqttManager with all required dependencies and configuration.
     * @param customWiFiManager Reference to the WiFi manager.
     * @param mqttManager Reference to the PubSubClient instance.
     * @param wifiClient Reference to the secure WiFi client.
     * @param server MQTT broker address.
     * @param port MQTT broker port.
     * @param username MQTT username.
     * @param password MQTT password.
     * @param deviceId Device identifier string.
     * @param dataTopic MQTT topic for sensor data.
     * @param statusTopic MQTT topic for device status.
     * @param jsonSerializer Reference to the JSON serializer.
     */
    MqttManager(CustomWiFiManager& customWiFiManager,
            PubSubClient& mqttManager,
            WiFiClientSecure& wifiClient,
            const char* server,
            int port,
            const char* username,
            const char* password,
            const char* deviceId,
            const char* dataTopic,
            const char* statusTopic,
            JsonSerializer& jsonSerializer); 
    
    /**
     * @brief Initializes the MQTT client and attempts initial connection.
     * @return true if connected, false otherwise.
     */
    bool setup();

    /**
     * @brief Main loop for MQTT handling. Reconnects and publishes status as needed.
     */
    void loop();

    /**
     * @brief Publishes sensor data to the MQTT broker.
     * @param temperature Temperature value.
     * @param humidity Humidity value.
     * @param gas CO2/gas value.
     * @param particles PM2.5 value.
     * @return true if published successfully, false otherwise.
     */
    bool publishSensorData(float temperature, float humidity, float gas, float particles);

    /**
     * @brief Clears a retained MQTT message on the specified topic.
     * @param topic The MQTT topic to clear.
     * @return true if cleared successfully, false otherwise.
     */
    bool clearRetainedMessage(const char* topic);

    int getMqttInterval() const {
        return mqttInterval;
    }

    void setMqttInterval (int interval) {
        mqttInterval = interval;
    }
    
private:
    CustomWiFiManager& _customWiFiManager;   // Reference to WiFi manager.
    PubSubClient& _mqttManager;              // Reference to PubSubClient.
    WiFiClientSecure& _wifiClient;           // Reference to secure WiFi client.
    
    const char* _server;                     // MQTT broker address.
    int _port;                               // MQTT broker port.
    const char* _username;                   // MQTT username.
    const char* _password;                   // MQTT password.
    const char* _deviceId;                   // Device identifier.
    const char* _dataTopic;                  // MQTT topic for sensor data.
    const char* _statusTopic;                // MQTT topic for device status.
    int mqttInterval = 300000;              // Interval for publishing status (5 minutes).
    const char* _topicDeviceUpdateInterval = "AirQuality/Server/UpdateInterval"; // Topic for update interval commands.
    
    JsonSerializer& _jsonSerializer;         // Reference to JSON serializer.

    String _clientId;                        // MQTT client ID.
    unsigned long _lastStatusUpdate;         // Last status publish time (ms).
    String _connectionTime;                  // Time of last successful connection.
    

    void onMqttMessage(char* topic, byte* payload, unsigned int length);

    /**
     * @brief Connects to the MQTT broker.
     * @return true if connected, false otherwise.
     */
    bool connect();

    /**
     * @brief Creates a JSON status message.
     * @param isConnected Whether the device is online.
     * @return JSON string representing the status.
     */
    String createStatusJson(bool isConnected);

    /**
     * @brief Creates a JSON sensor data message.
     * @param temperature Temperature value.
     * @param humidity Humidity value.
     * @param gas CO2/gas value.
     * @param particles PM2.5 value.
     * @return JSON string with sensor readings.
     */
    String createSensorJson(float temperature, float humidity, float gas, float particles);
};

#endif // MQTT_CLIENT_H