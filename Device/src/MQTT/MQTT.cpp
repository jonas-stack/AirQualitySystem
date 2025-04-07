#include "MQTT.h"
#include "config.h"  // Add this include

// Update these to use the config values
const char* ssid = WIFI_SSID;
const char* password = WIFI_PASSWORD;
const char* mqtt_server = MQTT_SERVER;
const int mqtt_port = MQTT_PORT;
const char* mqtt_username = MQTT_USERNAME;
const char* mqtt_password = MQTT_PASSWORD;

// Topic to publish sensor data
const char* sensor_topic = MQTT_TOPIC;

// Use the certificate from config.h
const char* rootCACertificate = ROOT_CA_CERTIFICATE;

// Create instances of WiFiClientSecure and PubSubClient
WiFiClientSecure espClient;
PubSubClient* client;

// Function to set up WiFi connection
void setupWiFi() {
  delay(10);
  Serial.print("Connecting to WiFi: ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  // Wait for WiFi connection
  unsigned long startAttemptTime = millis();
  while (WiFi.status() != WL_CONNECTED && millis() - startAttemptTime < 10000) {
    delay(500);
    Serial.print(".");
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("\nWiFi connected");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
  } else {
    Serial.println("\nWiFi connection failed");
  }
}

// Function to set date and time via NTP
void setDateTime() {
  Serial.print("Setting time using NTP: ");
  configTime(0, 0, "pool.ntp.org", "time.nist.gov");
  
  time_t now = time(nullptr);
  int retries = 0;
  while (now < 8 * 3600 * 2 && retries < 20) {
    delay(500);
    Serial.print(".");
    now = time(nullptr);
    retries++;
  }
  
  if (now < 8 * 3600 * 2) {
    Serial.println("Failed to set time via NTP");
    return;
  }
  
  Serial.println(" done");
  struct tm timeinfo;
  gmtime_r(&now, &timeinfo);
  Serial.print("Current UTC time: ");
  Serial.println(asctime(&timeinfo));
}

// MQTT message callback function
void mqttCallback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message received [");
  Serial.print(topic);
  Serial.print("]: ");
  
  char message[length + 1];
  for (unsigned int i = 0; i < length; i++) {
    message[i] = (char)payload[i];
    Serial.print((char)payload[i]);
  }
  message[length] = '\0';
  Serial.println();
  
  // Process incoming messages if needed
  // For example, you could parse JSON commands here using ArduinoJson
  
  // Example: Flash LED when message received
  digitalWrite(LED_BUILTIN, LOW);
  delay(100);
  digitalWrite(LED_BUILTIN, HIGH);
}

// Function to reconnect to MQTT broker
bool reconnectMQTT() {
  // Loop until we're reconnected or timeout
  unsigned long startAttemptTime = millis();
  
  while (!client->connected() && millis() - startAttemptTime < 5000) {
    Serial.print("Attempting MQTT connection...");
    
    // Create a client ID
    String clientId = "ESP32-AirQuality-";
    clientId += String(random(0xffff), HEX);
    
    // Attempt to connect with username/password
    if (client->connect(clientId.c_str(), mqtt_username, mqtt_password)) {
      Serial.println("connected");
      
      // Once connected, subscribe to any needed topics
      client->subscribe("airquality/commands");
      
      return true;
    } else {
      Serial.print("failed, rc=");
      Serial.print(client->state());
      Serial.println(" retrying in 1 second");
      delay(1000);
    }
  }
  
  return false;
}

// Initialize MQTT client and connection
bool setupMQTTClient() {
  // Initialize SPIFFS for certificate storage if needed
  if (!SPIFFS.begin(true)) {
    Serial.println("SPIFFS initialization failed");
    return false;
  }
  
  // Set up WiFi connection
  setupWiFi();
  if (WiFi.status() != WL_CONNECTED) {
    return false;
  }
  
  // Set date and time (needed for SSL certificate validation)
  setDateTime();
  
  // Configure SSL with certificate
  espClient.setCACert(rootCACertificate);
  
  // Create MQTT client
  client = new PubSubClient(espClient);
  client->setServer(mqtt_server, mqtt_port);
  client->setCallback(mqttCallback);
  client->setBufferSize(1024); // Increase buffer size for larger messages
  
  // Try to connect
  bool success = reconnectMQTT();
  if (success) {
    return true;
  } else {
    Serial.println("Failed to connect to MQTT broker");
    return false;
  }
}

// MQTT client maintenance
void loopMQTTClient() {
  // Check if client is still connected
  if (!client->connected()) {
    reconnectMQTT();
  }
  
  // Process MQTT messages
  client->loop();
}

// Publish sensor data to MQTT broker
bool publishSensorData(float temperature, float humidity, float gas, float particles) {
  if (!client->connected()) {
    if (!reconnectMQTT()) {
      return false;
    }
  }
  
  // Create JSON document for sensor data
  StaticJsonDocument<256> doc;
  doc["temperature"] = temperature;
  doc["humidity"] = humidity;
  doc["air_quality"] = gas;
  doc["pm25"] = particles;
  doc["device_id"] = DEVICE_ID;
  doc["timestamp"] = time(nullptr);
  
  // Serialize JSON to string
  char jsonBuffer[256];
  serializeJson(doc, jsonBuffer);
  
  // Publish to MQTT topic
  bool success = client->publish(sensor_topic, jsonBuffer, true);
  
  if (success) {
    Serial.println("Sensor data published to MQTT");
  } else {
    Serial.println("Failed to publish sensor data");
  }
  
  return success;
}