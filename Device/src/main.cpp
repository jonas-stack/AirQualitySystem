#include <Arduino.h>
#include <Wire.h>
#include "devices/bme280_sensor.h"
#include "devices/mq135_sensor.h"
#include "devices/lcd_display.h"
#include "devices/pm25_sensor.h"
#include "MQTT/MqttManager.h"
#include "WiFi/CustomWiFiManager.h"
#include "MQTT/TimeManager.h"
#include "MQTT/config.h"
#include "MQTT/JsonSerializer.h"

// Define WiFi reset button pin
#define WIFI_RESET_BUTTON_PIN 25

// Global topic definitions
const char* MQTT_DATA_TOPIC = "AirQuality/Data";
const char* MQTT_STATUS_TOPIC = "airquality/status";

// Constructors for classes
CustomWiFiManager customWiFiManager(WIFI_SSID, WIFI_PASSWORD, WIFI_AP_NAME, WIFI_AP_PASSWORD, WIFI_RESET_BUTTON_PIN);
WiFiClientSecure wifiClient;  
PubSubClient pubSubClient(wifiClient);
TimeManager timeManager;
JsonSerializer jsonSerializer;
MqttManager mqttClient(
    customWiFiManager,  
    timeManager,        
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

// Global timer variables
unsigned long previousMillis = 0;
const unsigned long interval = 3000;  // 3 seconds between readings

// MQTT timing
unsigned long lastMqttPublish = 0;
const unsigned long mqttInterval = 300000;  // 5 minutes between MQTT publishes

// configportal state status for non-blocking mode
bool configPortalActive = false;
unsigned long lastLcdUpdate = 0;

void setup() {
  Serial.begin(115200);
  delay(2000);
  pinMode(WIFI_RESET_BUTTON_PIN, INPUT_PULLUP);
  
  pinMode(2, OUTPUT);
  digitalWrite(2, HIGH);
  delay(300);
  digitalWrite(2, LOW);
  
  Serial.println("AIR QUALITY SYSTEM v1.0");
  
  Wire.begin();
  bool success = true;
  
  if (!customWiFiManager.connect()) success = false;
  timeManager.syncNTP();
  if (!setupBME280Sensor()) success = false;
  if (!setupLCDDisplay()) success = false;
  if (!setupMQ135Sensor()) success = false;
  if (!setupPM25Sensor()) success = false;

  mqttClient.setup();
  mqttClient.clearRetainedMessage(MQTT_DATA_TOPIC);
  
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

void loop() {
  unsigned long currentMillis = millis();

  // Check for button press to start config portal
  if (digitalRead(WIFI_RESET_BUTTON_PIN) == LOW && !configPortalActive) {
    Serial.println("Button pressed! Starting WiFi config portal...");
    configPortalActive = true;
    customWiFiManager.startConfigPortal();
    // After this, configPortalActive remains true until WiFi connects
  }

  // If config portal is active, delegate handling to CustomWiFiManager
  if (configPortalActive) {
    customWiFiManager.handleConfigPortal();

    // Exit portal mode if WiFi is connected
    if (customWiFiManager.isConnected()) {
      configPortalActive = false;
      displayMessage("WiFi Connected", "Yes");
      Serial.println("WiFi connected successfully!" + customWiFiManager.isConnected());
      delay(2000);
    }

    // Don't run the rest of the loop while in config portal
    return;
  }

  mqttClient.loop();
  customWiFiManager.loop();
  

  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    
    float temp = readBME280Temperature();
    float humidity = readBME280Humidity();
    float gas = readMQ135Sensor();
    float particles = readPM25Value();
    updateLCDDisplay(temp, humidity, gas, particles);

    if (currentMillis - lastMqttPublish >= mqttInterval) {
      lastMqttPublish = currentMillis;
      mqttClient.publishSensorData(temp, humidity, gas, particles);
    }
    
    digitalWrite(2, HIGH);
    delay(50);
    digitalWrite(2, LOW);
  }
  
  delay(10);
}