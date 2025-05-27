#include <Arduino.h>
#include <Wire.h>
#include "devices/bme280_sensor.h"
#include "devices/mq135_sensor.h"
#include "devices/lcd_display.h"
#include "devices/pm25_sensor.h"
#include "MQTT/MqttManager.h"
#include "WiFi/CustomWiFiManager.h"
#include "MQTT/config.h"
#include "MQTT/JsonSerializer.h"


#define WIFI_RESET_BUTTON_PIN 25   // Pin for WiFi reset/config button

const char* MQTT_DATA_TOPIC = "AirQuality/Data";
const char* MQTT_STATUS_TOPIC = "airquality/status";

// -------------------------
// Global Object Instantiations
// -------------------------
CustomWiFiManager customWiFiManager(WIFI_SSID, WIFI_PASSWORD, WIFI_AP_NAME, WIFI_AP_PASSWORD, WIFI_RESET_BUTTON_PIN);
WiFiClientSecure wifiClient;  
PubSubClient pubSubClient(wifiClient);
JsonSerializer jsonSerializer;
MqttManager mqttClient(
    customWiFiManager,        
    pubSubClient,       
    wifiClient,        
    MQTT_SERVER,
    MQTT_PORT,
    MQTT_USERNAME,
    MQTT_PASSWORD,
    DEVICE_ID,
    MQTT_DATA_TOPIC,
    MQTT_STATUS_TOPIC,
    jsonSerializer
);


unsigned long previousMillis = 0;           // For sensor reading interval
const unsigned long interval = 3000;        // 3 seconds between sensor readings

unsigned long lastMqttPublish = 0;          // For MQTT publish interval
const unsigned long mqttInterval = 300000;  // 5 minutes between MQTT publishes


bool configPortalActive = false;            // Tracks if config portal is active

// -------------------------
// Arduino Setup Function
// -------------------------
void setup() {
  Serial.begin(115200);
  delay(2000);

  // Initialize hardware pins
  pinMode(WIFI_RESET_BUTTON_PIN, INPUT_PULLUP); // Button for WiFi reset/config
  pinMode(2, OUTPUT); // Status LED

  // Blink LED to indicate startup
  digitalWrite(2, HIGH);
  delay(300);
  digitalWrite(2, LOW);

  Serial.println("AIR QUALITY SYSTEM v1.0");

  Wire.begin(); // Initialize I2C bus

  bool success = true;

  // Connect to WiFi
  if (!customWiFiManager.connect()) success = false;

  // Synchronize time via NTP (UTC+2)
  configTime(7200, 0, "pool.ntp.org");
  unsigned long startTime = millis();
  while (time(nullptr) < 1600000000) { // Wait for valid time (after 2020)
    delay(100);
    if (millis() - startTime > 30000) {
      Serial.println("NTP SYNC FAILED");
      success = false;
      break;
    }
  }

  // Initialize sensors and display
  if (!setupBME280Sensor())   { Serial.println("BME280 init failed!");   success = false; }
  if (!setupLCDDisplay())     { Serial.println("LCD init failed!");      success = false; }
  if (!setupMQ135Sensor())    { Serial.println("MQ135 init failed!");    success = false; }
  if (!setupPM25Sensor())     { Serial.println("PM2.5 init failed!");    success = false; }

  // Initialize MQTT and clear retained data
  mqttClient.setup();
  mqttClient.clearRetainedMessage(MQTT_DATA_TOPIC);

  // Halt system if any initialization failed
  if (!success) {
    Serial.println("INITIALIZATION FAILED - SYSTEM HALTED");
    while (true) {
      digitalWrite(2, HIGH);
      delay(200);
      digitalWrite(2, LOW);
      delay(200);
    }
  }

  Serial.println("SYSTEM INITIALIZED");
}

// -------------------------
// Arduino Main Loop
// -------------------------
void loop() {
  unsigned long currentMillis = millis();

  // Handle WiFi config portal button press
  if (digitalRead(WIFI_RESET_BUTTON_PIN) == LOW && !configPortalActive) {
    Serial.println("Button pressed! Starting WiFi config portal...");
    configPortalActive = true;
    customWiFiManager.startConfigPortal();
    // Remain in config portal mode until WiFi connects
  }

  // If config portal is active, handle portal logic and block main loop
  if (configPortalActive) {
    customWiFiManager.handleConfigPortal();

    // Exit portal mode if WiFi is connected
    if (customWiFiManager.isConnected()) {
      configPortalActive = false;
      displayMessage("WiFi Connected", "Yes");
      Serial.print("WiFi connected successfully! ");
      Serial.println(customWiFiManager.isConnected() ? "true" : "false");
      delay(2000);
    }
    return; // Skip rest of loop while in config portal
  }

  // Regular operation: handle MQTT and WiFi background tasks
  mqttClient.loop();
  customWiFiManager.loop();

  // Sensor reading and MQTT publishing intervals
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;

    // Read sensors
    float temp = readBME280Temperature();
    float humidity = readBME280Humidity();
    float gas = readMQ135Sensor();
    float particles = readPM25Value();

    // Update LCD display
    updateLCDDisplay(temp, humidity, gas, particles);

    // Publish sensor data to MQTT at defined interval
    if (currentMillis - lastMqttPublish >= mqttClient.getMqttInterval()) {
      lastMqttPublish = currentMillis;
      mqttClient.publishSensorData(temp, humidity, gas, particles);
    }

    // Blink status LED to indicate activity
    digitalWrite(2, HIGH);
    delay(50);
    digitalWrite(2, LOW);
  }

  delay(10);
}