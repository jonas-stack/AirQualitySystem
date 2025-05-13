#include "ConnectionManager.h"

ConnectionManager::ConnectionManager(WiFiManager* wifiManager, 
                                   const char* server, 
                                   int port,
                                   const char* username,
                                   const char* password) :
    _wifiManager(wifiManager),
    _server(server),
    _port(port),
    _username(username),
    _password(password), 
    _configured(false) {
    
    // Generate a random client ID
    _clientId = "ESP32Client-";
    _clientId += String(random(0xffff), HEX);
}

ConnectionManager::~ConnectionManager() {
    if (_client) {
        delete _client;
        _client = nullptr;
    }
}

bool ConnectionManager::setup() {
    // Set insecure client (for development purposes)
    _espClient.setInsecure();
    
    // Create and configure MQTT client
    _client = new PubSubClient(_espClient);
    _client->setServer(_server, _port);
    _client->setBufferSize(1024); // Increase buffer size for larger messages
    
    _configured = true;
    return true;
}

void ConnectionManager::loop() {
    if (!_configured || !_client) return;
    
    _client->loop();
}

bool ConnectionManager::isConnected() {
    return _client && _client->connected();
}

bool ConnectionManager::connect(const char* willTopic, 
                              int willQos, 
                              bool willRetain, 
                              const char* willMessage) {
    if (!_configured || !_client) return false;
    if (!_wifiManager->isConnected()) {
        Serial.println("WiFi not connected. Cannot connect to MQTT broker.");
        return false;
    }
    
    Serial.print("Attempting MQTT connection...");
    
    bool connected = false;
    
    // Connect with or without LWT based on parameters
    if (willTopic && willMessage) {
        connected = _client->connect(_clientId.c_str(), 
                                   _username, 
                                   _password,
                                   willTopic,
                                   willQos,
                                   willRetain,
                                   willMessage);
    } else {
        connected = _client->connect(_clientId.c_str(), _username, _password);
    }
    
    if (connected) {
        Serial.println("connected to MQTT broker");
    } else {
        Serial.print("failed, rc=");
        Serial.print(_client->state());
    }
    
    return connected;
}

void ConnectionManager::disconnect() {
    if (_client && _client->connected()) {
        _client->disconnect();
    }
}

bool ConnectionManager::publish(const char* topic, const char* payload, bool retain) {
    if (!isConnected()) {
        Serial.println("Not connected to MQTT broker. Cannot publish.");
        return false;
    }
    
    bool success = _client->publish(topic, payload, retain);
    
    if (success) {
        Serial.print("Message published to ");
        Serial.println(topic);
    } else {
        Serial.print("Failed to publish to ");
        Serial.println(topic);
    }
    
    return success;
}