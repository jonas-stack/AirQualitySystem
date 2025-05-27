#ifndef BME280_SENSOR_H
#define BME280_SENSOR_H

#include <Arduino.h>
#include <Adafruit_BME280.h>

/**
 * @brief Initializes the BME280 sensor.
 * Tries both common I2C addresses (0x76 and 0x77).
 * @return true if the sensor is found and initialized, false otherwise.
 */
bool setupBME280Sensor();

/**
 * @brief Reads and returns the current temperature from the BME280 sensor.
 * @return Temperature in degrees Celsius.
 */
float readBME280Temperature();

/**
 * @brief Reads and returns the current humidity from the BME280 sensor.
 * @return Relative humidity in percent.
 */
float readBME280Humidity();

/**
 * @brief Reads and returns the current pressure from the BME280 sensor.
 * @return Pressure in hPa (hectopascals).
 */
float readBME280Pressure();

/**
 * @brief Updates the global variables with the latest sensor readings.
 */
void updateBME280SensorValues();

#endif // BME280_SENSOR_H