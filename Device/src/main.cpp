#include <Arduino.h>
#include <Wire.h>
#include "bme280_sensor.h"
#include "mq2_sensor.h"
#include "lcd_display.h"
#include "pm25_sensor.h"

// Global timer variables
unsigned long previousMillis = 0;
const unsigned long interval = 3000;  // 3 seconds between readings

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
  
  Serial.print("BME280 sensor: ");
  if (setupBME280Sensor()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }
  delay(500);  // Small delay between initializations
  
  Serial.print("LCD display: ");
  if (setupLCDDisplay()) {
    Serial.println("OK");
  } else {
    Serial.println("FAILED");
    success = false;
  }
  delay(500);
  
  Serial.print("MQ2 sensor: ");
  if (setupMQ2Sensor()) {
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

  // Read sensor data at regular intervals
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    
    Serial.println("\n====== SENSOR READINGS ======");
    
    // Read and display all sensor data with small delays for readability
    Serial.println("\n>> BME280 ENVIRONMENTAL DATA:");
    printBME280SensorData();
    delay(100);
    
    Serial.println("\n>> MQ2 GAS SENSOR DATA:");
    printMQ2SensorData();
    delay(100);
    
    Serial.println("\n>> PM2.5 PARTICULATE DATA:");
    printPM25SensorData();
    delay(100);
    
    // Update LCD display with current readings
    float temp = readBME280Temperature();
    float humidity = readBME280Humidity();
    float gas = readMQ2Sensor();
    updateLCDDisplay(temp, humidity, gas);
    
    // Blink LED once to indicate successful reading cycle
    digitalWrite(2, HIGH);
    delay(50);
    digitalWrite(2, LOW);
    
    Serial.println("\n====== END OF READINGS ======");
  }
  
  // Small yield to allow other processes to run
  delay(10);
}