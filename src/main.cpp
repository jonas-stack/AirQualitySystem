#include <Arduino.h>
#include <Wire.h>
#include "bme280_sensor.h"
#include "mq2_sensor.h"

// Global timer variables
unsigned long previousMillis = 0;
const unsigned long interval = 3000;  // 3 seconds between readings

void setup() {
  Serial.begin(115200);
  Serial.println("Air Quality system starting...");

  pinMode(2, OUTPUT);  // Built-in LED on most ESP32 boards

  Wire.begin();

  if (!setupBME280Sensor()) {
    Serial.println("Could not initialize the BME280 sensor, check wiring!");
    while (true);
  }
}

void loop() {
  unsigned long currentMillis = millis();

  // Check if 10 seconds have passed since last reading
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;  // Save the current time
    printBME280SensorData();
    printMQ2SensorData();
  }
  
  // Blink built-in LED to see if the system is running
  digitalWrite(2, HIGH);
  delay(1000);
  digitalWrite(2, LOW);
  delay(1000);
}