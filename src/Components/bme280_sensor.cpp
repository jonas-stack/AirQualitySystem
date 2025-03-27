#include "bme280_sensor.h"

#define SEALEVELPRESSURE_HPA (1013.25)

Adafruit_BME280 bme; // I2C

bool setupBME280Sensor() {
  // The BME280 has default I2C address 0x76 (sometimes 0x77)
  bool status = bme.begin(0x76);
  
  if (!status) {
    Serial.println("Could not find a valid BME280 sensor, check wiring or try address 0x77!");
    return false;
  }
  
  // Weather monitoring settings for normal operation
  bme.setSampling(Adafruit_BME280::MODE_NORMAL,    // Operating Mode
                  Adafruit_BME280::SAMPLING_X1,    // Temp. oversampling
                  Adafruit_BME280::SAMPLING_X1,    // Pressure oversampling
                  Adafruit_BME280::SAMPLING_X1,    // Humidity oversampling
                  Adafruit_BME280::FILTER_OFF,     // Filtering
                  Adafruit_BME280::STANDBY_MS_1000); // Standby time
  
  Serial.println("BME280 sensor initialized successfully");
  return true;
}

float readBME280Temperature() {
  return bme.readTemperature(); // Returns temperature in Celsius
}

float readBME280Humidity() {
  return bme.readHumidity(); // Returns humidity in %
}

float readBME280Pressure() {
  return bme.readPressure() / 100.0F; // Convert Pa to hPa (millibar)
}