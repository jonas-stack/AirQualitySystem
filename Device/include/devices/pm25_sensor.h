#ifndef PM25_SENSOR_H
#define PM25_SENSOR_H

#include <Arduino.h>
#include <PMS.h>
#include <HardwareSerial.h>

/**
 * @brief Initializes the PM2.5 sensor hardware and serial communication.
 * @return true if initialization succeeds, false otherwise.
 */
bool setupPM25Sensor();

/**
 * @brief Reads and returns the latest PM2.5 value from the sensor.
 * @return PM2.5 concentration in micrograms per cubic meter.
 */
float readPM25Value();

/**
 * @brief Updates the global variables with the latest PM sensor readings.
 */
void updatePM25SensorValues();

#endif // PM25_SENSOR_H