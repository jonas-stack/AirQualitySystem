#include "ConnectionManager.h"
#include "WiFi/CustomWiFiManager.h"

ConnectionManager::ConnectionManager(CustomWiFiManager& customWiFiManager,
                                     PubSubClient& client,
                                     WiFiClientSecure& espClient,
                                     const char* server,
                                     int port,
                                     const char* username,
                                     const char* password)
    : _customWiFiManager(customWiFiManager),
      _client(client),
      _espClient(espClient),
      _server(server),
      _port(port),
      _username(username),
      _password(password),
      _configured(false)
{
    _clientId = "ESP32Client-";
    _clientId += String(random(0xffff), HEX);
}

bool ConnectionManager::setup() {
    _espClient.setInsecure();
    _client.setServer(_server, _port);
    _client.setBufferSize(1024);
    _configured = true;
    return true;
}

void ConnectionManager::loop() {
    if (!_configured) return;
    _client.loop();
}

bool ConnectionManager::isConnected() {
    return _client.connected();
}

bool ConnectionManager::connect(const char* willTopic,
                                int willQos,
                                bool willRetain,
                                const char* willMessage) {
    if (!_configured) return false;
    if (!_customWiFiManager.isConnected()) {
        Serial.println("WiFi not connected. Cannot connect to MQTT broker.");
        return false;
    }

    Serial.print("Attempting MQTT connection...");

    bool connected = false;

    if (willTopic && willMessage) {
        connected = _client.connect(_clientId.c_str(),
                                    _username,
                                    _password,
                                    willTopic,
                                    willQos,
                                    willRetain,
                                    willMessage);
    } else {
        connected = _client.connect(_clientId.c_str(), _username, _password);
    }

    if (connected) {
        Serial.println("connected to MQTT broker");
    } else {
        Serial.print("failed, rc=");
        Serial.print(_client.state());
    }

    return connected;
}

void ConnectionManager::disconnect() {
    if (_client.connected()) {
        _client.disconnect();
    }
}

bool ConnectionManager::publish(const char* topic, const char* payload, bool retain) {
    if (!isConnected()) {
        Serial.println("Not connected to MQTT broker. Cannot publish.");
        return false;
    }

    bool success = _client.publish(topic, payload, retain);

    if (success) {
        Serial.print("Message published to ");
        Serial.println(topic);
    } else {
        Serial.print("Failed to publish to ");
        Serial.println(topic);
    }

    return success;
}