#include "MQTT/MqttManager.h"
#include "JsonSerializer.h"
#include "WiFi/CustomWiFiManager.h"

/**
 * Constructs an MqttManager with all required dependencies and configuration.
 */
MqttManager::MqttManager(
    CustomWiFiManager& customWiFiManager,
    PubSubClient& mqttManager,
    WiFiClientSecure& wifiClient,
    const char* server,
    int port,
    const char* username,
    const char* password,
    const char* deviceId,
    const char* dataTopic,
    const char* statusTopic,
    JsonSerializer& jsonSerializer
) :
    _customWiFiManager(customWiFiManager),
    _mqttManager(mqttManager),
    _wifiClient(wifiClient),
    _server(server),
    _port(port),
    _username(username),
    _password(password),
    _deviceId(deviceId),
    _dataTopic(dataTopic),
    _statusTopic(statusTopic),
    _jsonSerializer(jsonSerializer),
    _lastStatusUpdate(0)
{
    // Generate a unique MQTT client ID, for connecting to the broker.
    _clientId = "ESP32Client-";
    _clientId += String(random(0xffff), HEX);
}

/**
 * Initializes the MQTT client and attempts initial connection.
 * @return true if connected, false otherwise.
 */
bool MqttManager::setup() {
    _wifiClient.setInsecure();
    _mqttManager.setServer(_server, _port);
    _mqttManager.setBufferSize(1024);
    _mqttManager.setCallback([this](char* topic, byte* payload, unsigned int length) {
        this->onMqttMessage(topic, payload, length);
    });

    return connect();
}

/**
 * Connects to the MQTT broker.
 * Sets up the Last Will and Testament (LWT) message.
 * Publishes an "online" status message upon successful connection.
 * @return true if connected, false otherwise.
 */
bool MqttManager::connect() {
    if (!_customWiFiManager.isConnected()) {
        return false;
    }

    // Prepare offline message for LWT.
    String offlineMsg = _jsonSerializer.serializeStatusMessage("offline", _deviceId, this->getMqttInterval());

    bool connected = _mqttManager.connect(
        _clientId.c_str(), 
        _username, 
        _password,
        _statusTopic,
        1,
        true,
        offlineMsg.c_str()
    );

    if (connected) {
        _connectionTime = time(nullptr);
        String onlineMsg = this->createStatusJson(connected);
        _mqttManager.publish(_statusTopic, onlineMsg.c_str(), true);

        _mqttManager.subscribe(_topicDeviceUpdateInterval); // Subscribe to command topic
    }

    return connected;
}

/**
 * Main loop for MQTT handling.
 * Reconnects if disconnected and periodically publishes status.
 */
void MqttManager::loop() {
    if (!_mqttManager.connected()) {
        connect();
    }
    
    _mqttManager.loop();
    
    // Publish status every 5 minutes.
    unsigned long now = millis();
    if (now - _lastStatusUpdate > 300000) {
        _lastStatusUpdate = now;
        if (_mqttManager.connected()) {
            String statusMsg = createStatusJson(true);
            _mqttManager.publish(_statusTopic, statusMsg.c_str(), true);
        }
    }
}

void MqttManager::onMqttMessage(char* topic, byte* payload, unsigned int length) {
    // Convert payload to String
    String message;
    for (unsigned int i = 0; i < length; i++) {
        message += (char)payload[i];
    }
    // Handle the message as needed
    Serial.print("Received message on topic: ");
    Serial.print(topic);
    Serial.print(" -> ");
    Serial.println(message);

    if (String(topic) == _topicDeviceUpdateInterval) {
        DynamicJsonDocument doc(256);
        DeserializationError error = deserializeJson(doc, message);
        if (error) {
            Serial.print("Failed to parse JSON: ");
            Serial.println(error.c_str());
            return;
        }
        if (!error && doc.containsKey("interval")) {
            int newInterval = doc["interval"];
            if (newInterval >= 10000) { // Minimum 10 seconds
                setMqttInterval(newInterval);
                Serial.print("MQTT publish interval updated to: ");
                Serial.println(getMqttInterval());
            }
        }
    }
}

/**
 * Creates a JSON status message using the JsonSerializer.
 * @param isConnected Whether the device is online.
 * @return JSON string representing the status.
 */
String MqttManager::createStatusJson(bool isConnected) {
    return _jsonSerializer.serializeStatusMessage(isConnected ? "online" : "offline", _deviceId, this->getMqttInterval());
}

/**
 * Creates a JSON sensor data message using the JsonSerializer.
 * @return JSON string with sensor readings.
 */
String MqttManager::createSensorJson(float temperature, float humidity, float gas, float particles) {
    return _jsonSerializer.serializeSensorData(temperature, humidity, gas, particles, _deviceId);
}

/**
 * Publishes sensor data to the MQTT broker.
 * Connects if not already connected.
 * @return true if published successfully, false otherwise.
 */
bool MqttManager::publishSensorData(float temperature, float humidity, float gas, float particles) {
    if (!_mqttManager.connected()) {
        if (!connect()) {
            return false;
        }
    }
    
    String jsonString = createSensorJson(temperature, humidity, gas, particles);
    Serial.print("Publishing sensor data: ");
    Serial.println(jsonString);
    return _mqttManager.publish(_dataTopic, jsonString.c_str(), false);
}

/**
 * Clears a retained MQTT message on the specified topic.
 * Connects if not already connected.
 * @param topic The MQTT topic to clear.
 * @return true if cleared successfully, false otherwise.
 */
bool MqttManager::clearRetainedMessage(const char* topic) {
    if (!_mqttManager.connected()) {
        if (!connect()) {
            return false;
        }
    }
    return _mqttManager.publish(topic, "", true);
}