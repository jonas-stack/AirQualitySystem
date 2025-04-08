#include "MQTT.h"  
#include "MQTT/config_template.h"
#include "JsonSerializer.h"  

// Update these to use the config values
const char* ssid = WIFI_SSID;
const char* password = WIFI_PASSWORD;
const char* mqtt_server = MQTT_SERVER;
const int mqtt_port = MQTT_PORT;
const char* mqtt_username = MQTT_USERNAME;
const char* mqtt_password = MQTT_PASSWORD;

// Topic to publish sensor data
const char* sensor_topic = MQTT_TOPIC;

// Create instances of WiFiClientSecure and PubSubClient
WiFiClientSecure espClient;
PubSubClient* client;
unsigned long lastMsg = 0;

// Function to set up WiFi connection
void setupWiFi() {
  delay(10);
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

// Function to set date and time via NTP
void setDateTime() {
  Serial.print("Waiting for NTP time sync: ");
  configTime(0, 0, "pool.ntp.org", "time.nist.gov");
  
  time_t now = time(nullptr);
  while (now < 8 * 3600 * 2) {
    delay(100);
    Serial.print(".");
    now = time(nullptr);
  }
  Serial.println();

  struct tm timeinfo;
  gmtime_r(&now, &timeinfo);
  Serial.print("Current UTC time: ");
  Serial.println(asctime(&timeinfo));
}

// MQTT message callback function
void mqttCallback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  
  // Print the payload content
  for (unsigned int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();

  // Flash LED when message received
  if (length > 0) {
    digitalWrite(LED_BUILTIN, LOW);
    delay(500);
    digitalWrite(LED_BUILTIN, HIGH);
  }
}

// Function to reconnect to MQTT broker
void reconnectMQTT() {
  // Loop until we're reconnected
  while (!client->connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP32Client-";
    clientId += String(random(0xffff), HEX);
    
    // Attempt to connect with username/password
    if (client->connect(clientId.c_str(), mqtt_username, mqtt_password)) {
      Serial.println("connected");
      // Once connected, subscribe to any needed topics
      client->subscribe("airquality/commands");
      
      // Publish a connection announcement
      client->publish("airquality/status", "Device online", true);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client->state());
      Serial.println(" try again in 5 seconds");
      // Wait before retrying
      delay(5000);
    }
  }
}

// Initialize MQTT client and connection
bool setupMQTTClient() {
  pinMode(LED_BUILTIN, OUTPUT); // Initialize the LED_BUILTIN pin as an output
  
  // Set up WiFi connection
  setupWiFi();
  setDateTime();
  
  // For ESP32, we can simply use setInsecure() for development purposes
  espClient.setInsecure();
  
  client = new PubSubClient(espClient);
  client->setServer(mqtt_server, mqtt_port);
  client->setCallback(mqttCallback);
  client->setBufferSize(1024); // Increase buffer size for larger messages
  
  return true;
}

// MQTT client maintenance
void loopMQTTClient() {
  if (!client->connected()) {
    reconnectMQTT();
  }
  client->loop();
}

// Publish sensor data to MQTT broker
bool publishSensorData(float temperature, float humidity, float gas, float particles) {
  if (!client->connected()) {
    reconnectMQTT();
  }
  
  // Use the JsonSerializer to create the JSON string
  String jsonString = JsonSerializer::serializeSensorData(temperature, humidity, gas, particles, DEVICE_ID);
  
  // Publish to MQTT topic
  bool success = client->publish(sensor_topic, jsonString.c_str(), true);
  
  if (success) {
    Serial.println("Sensor data published to MQTT");
  } else {
    Serial.println("Failed to publish sensor data");
  }
  
  return success;
}