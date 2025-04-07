#ifndef PM25_SENSOR_H
#define PM25_SENSOR_H

#include <Arduino.h>
#include <PMS.h>
#include <HardwareSerial.h>

bool setupPM25Sensor();
void printPM25SensorData();
float getPM25Value();

#endif