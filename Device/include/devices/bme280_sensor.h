#ifndef BME280_SENSOR_H
#define BME280_SENSOR_H

#include <Arduino.h>
#include <Adafruit_BME280.h>

bool setupBME280Sensor();
float readBME280Temperature();
float readBME280Humidity();
float readBME280Pressure();
void updateBME280SensorValues();
void printBME280SensorData();

#endif