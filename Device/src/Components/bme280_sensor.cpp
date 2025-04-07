#include "bme280_sensor.h"

#define SEALEVELPRESSURE_HPA (1013.25)

Adafruit_BME280 bme; // I2C

bool setupBME280Sensor() {
  Serial.println("Initializing BME280 sensor...");
  
  // Try address 0x76 first
  bool status = bme.begin(0x76);
  
  if (!status) {
    // If that fails, try address 0x77
    status = bme.begin(0x77);
  }
 
  if (!status) {
    Serial.println("BME280 sensor initialization failed!");
    return false;
  }
  
  Serial.println("BME280 sensor initialized successfully.");
  return true;
  delay(5000);
}

float readBME280Temperature() {
  return bme.readTemperature(); // Returns temperature in °C
}

float readBME280Humidity() {
  return bme.readHumidity(); // Returns humidity in %
}

float readBME280Pressure() {
  return bme.readPressure() / 100.0F; // Convert Pa to hPa (millibar)
}

void printBME280SensorData() {
  float temperature = readBME280Temperature();
  float humidity = readBME280Humidity();
  float pressure = readBME280Pressure();
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
}