#ifndef MQ135_SENSOR_H
#define MQ135_SENSOR_H

#include <Arduino.h>
#include <MQUnifiedsensor.h>
#include <WiFi.h>

/**
 * @brief Initializes and calibrates the MQ135 sensor for CO2 detection.
 * @return true if initialization and calibration succeed, false otherwise.
 */
bool setupMQ135Sensor();

/**
 * @brief Reads and returns the latest corrected CO2 value from the MQ135 sensor.
 * @return CO2 concentration in ppm.
 */
float readMQ135Sensor();

/**
 * @brief Updates all global sensor values from the MQ135 sensor.
 */
void updateSensorValues();

#endif // MQ135_SENSOR_H