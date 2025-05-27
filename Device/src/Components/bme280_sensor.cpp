#include "devices/bme280_sensor.h"

#define SEALEVELPRESSURE_HPA (1013.25) // Standard sea level pressure for altitude calculations

// BME280 sensor object (I2C interface).
Adafruit_BME280 bme;

// Global variables to store the latest sensor readings.
static float temperature = 0.0;
static float humidity = 0.0;
static float pressure = 0.0;

/**
 * Initializes the BME280 sensor.
 * Tries both common I2C addresses (0x76 and 0x77).
 * @return true if the sensor is found and initialized, false otherwise.
 */
bool setupBME280Sensor() {
    Serial.println("Initializing BME280 sensor...");
    
    // Try to initialize the BME280 sensor with the default I2C address 0x76.
    bool status = bme.begin(0x76);
    
    if (!status) {
        // If that fails, try address 0x77.
        status = bme.begin(0x77);
    }
 
    if (!status) {
        Serial.println("BME280 sensor initialization failed!");
        return false;
    }
    
    Serial.println("BME280 sensor initialized successfully.");
    return true;
}

/**
 * Updates the global variables with the latest sensor readings.
 */
void updateBME280SensorValues() {
    temperature = bme.readTemperature(); 
    humidity = bme.readHumidity(); 
    pressure = bme.readPressure() / 100.0F;
}

/**
 * Reads and returns the current temperature from the BME280 sensor.
 * @return Temperature in degrees Celsius.
 */
float readBME280Temperature() {
    return bme.readTemperature();
}

/**
 * Reads and returns the current humidity from the BME280 sensor.
 * @return Relative humidity in percent.
 */
float readBME280Humidity() {
    return bme.readHumidity();
}

/**
 * Reads and returns the current pressure from the BME280 sensor.
 * @return Pressure in hPa (hectopascals).
 */
float readBME280Pressure() {
    return bme.readPressure() / 100.0F;
}