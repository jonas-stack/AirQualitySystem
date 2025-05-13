#include <Arduino.h>
#include <Wire.h>
#include "devices/bme280_sensor.h"
#include "devices/mq135_sensor.h"
#include "devices/lcd_display.h"
#include "devices/pm25_sensor.h"
#include "MQTT/MQTTClient.h"
#include "MQTT/WiFiManager.h"
#include "MQTT/TimeManager.h"
#include "MQTT/config.h"

// Create class instances
WiFiManager wifiManager(WIFI_SSID, WIFI_PASSWORD);
TimeManager timeManager;
MQTTClient mqttClient(
    &wifiManager,
    &timeManager,
    MQTT_SERVER,
    MQTT_PORT,
    MQTT_USERNAME,
    MQTT_PASSWORD,
    DEVICE_ID
);

// Default MQTT callback function
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

// Global timer variables
unsigned long previousMillis = 0;
const unsigned long interval = 3000;  // 3 seconds between readings

// MQTT timing (publish less frequently to reduce network traffic)
unsigned long lastMqttPublish = 0;
const unsigned long mqttInterval = 300000;  // 5 minutes between MQTT publishes

void setup() {
  // Initialize serial communication at 115200 baud
  Serial.begin(115200);
  delay(2000);  // Longer wait for serial to initialize completely
  
  // Setup LED for status indication
  pinMode(2, OUTPUT);
  digitalWrite(2, HIGH);
  delay(300);
  digitalWrite(2, LOW);
  
  Serial.println("\n\n");
  Serial.println("=============================");
  Serial.println("   AIR QUALITY SYSTEM v1.0   ");
  Serial.println("=============================");
  
  Wire.begin();
  Serial.println("Starting sensor initialization sequence...");

  // Initialize sensors with basic error checking
  bool success = true;
  
  // Connect to WiFi
  Serial.print("WiFi connection: ");
  if (wifiManager.connect()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }
  delay(500);
  
  // Sync time
  Serial.print("NTP Time sync: ");
  if (timeManager.syncNTP()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
  }
  delay(500);
  
  Serial.print("BME280 sensor: ");
  if (setupBME280Sensor()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }
  delay(500);
  
  Serial.print("LCD display: ");
  if (setupLCDDisplay()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }
  delay(500);
  
  Serial.print("MQ135 sensor: ");
  if (setupMQ135Sensor()) {  
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }
  delay(500);
  
  Serial.print("PM2.5 sensor: ");
  if (setupPM25Sensor()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }

  delay(500);

  Serial.print("MQTT client: ");
  // Setup MQTT and set the callback
  mqttClient.setCallback(mqttCallback);
  if (mqttClient.setup()) {
    Serial.println("OK");
    
    // Clear retained messages
    Serial.println("Clearing retained MQTT messages...");
    mqttClient.clearRetainedMessage(MQTT_TOPIC); 
    Serial.println("Retained messages cleared");
    
  } else {
    Serial.println("FAILED - Will retry later");
    // Not considering this a critical failure
  }
  
  // Handle initialization failure
  if (!success) {
    Serial.println("\n=============================");
    Serial.println("INITIALIZATION FAILED - SYSTEM HALTED");
    Serial.println("Please check sensor connections and restart");
    Serial.println("=============================");
    
    // Error blink pattern (rapid blinking)
    while (true) {
      digitalWrite(2, HIGH);
      delay(200);
      digitalWrite(2, LOW);
      delay(200);
    }
  }

  // All sensors initialized successfully
  Serial.println("\n=============================");
  Serial.println("ALL SENSORS INITIALIZED SUCCESSFULLY");
  Serial.println("Starting monitoring loop...");
  Serial.println("=============================\n");
}

void loop() {
  unsigned long currentMillis = millis();

  mqttClient.loop();  // Handle MQTT client loop

  // Read sensor data at regular intervals
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    
    Serial.println("\n====== SENSOR READINGS ======");
    
    // Read and display all sensor data with small delays for readability
    Serial.println("\n>> BME280 ENVIRONMENTAL DATA:");
    printBME280SensorData();
    delay(100);
    
    Serial.println("\n>> MQ135 GAS SENSOR DATA:");
    printMQ135SensorData();
    delay(100);
    
    Serial.println("\n>> PM2.5 PARTICULATE DATA:");
    printPM25SensorData();
    delay(100);
    
    // Update LCD display with current readings
    float temp = readBME280Temperature();
    float humidity = readBME280Humidity();
    float gas = readMQ135Sensor();
    float particles = readPM25Value();
    updateLCDDisplay(temp, humidity, gas, particles);

    // Publish MQTT data at the longer interval
    if (currentMillis - lastMqttPublish >= mqttInterval) {
      lastMqttPublish = currentMillis;
      
      Serial.println("\n>> PUBLISHING TO MQTT:");
      bool published = mqttClient.publishSensorData(temp, humidity, gas, particles);
      if (published) {
        Serial.println("Data sent successfully");
      } else {
        Serial.println("Failed to send data");
      }
    }
    
    // Blink LED once to indicate successful reading cycle
    digitalWrite(2, HIGH);
    delay(50);
    digitalWrite(2, LOW);
    
    Serial.println("\n====== END OF READINGS ======");
  }
  
  // Small yield to allow other processes to run
  delay(10);
}