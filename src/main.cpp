#include <Arduino.h>
#include "mq2_sensor.h"
#include "bme280_sensor.h"
#include "pm25_sensor.h"
#include "lcd_display.h"

void setup() {
  // Initialize serial communication
  Serial.begin(115200);
  Serial.println("Air Quality System Starting...");
  
  // Initialize BME280 sensor
  if (setupBME280Sensor()) {
    Serial.println("BME280 ready!");
  } else {
    Serial.println("BME280 setup failed!");
  }
  
  // Additional setup code will go here later
}

void loop() {
  // Read BME280 data
  float temperature = readBME280Temperature();
  float humidity = readBME280Humidity();
  float pressure = readBME280Pressure();
  
  // Output data to serial monitor
  Serial.print("Temperature: ");
  Serial.print(temperature);
  Serial.println(" °C");
  
  Serial.print("Humidity: ");
  Serial.print(humidity);
  Serial.println(" %");
  
  Serial.print("Pressure: ");
  Serial.print(pressure);
  Serial.println(" hPa");
  
  Serial.println("---------------------");
  
  delay(2000); // Wait 2 seconds before next reading
}