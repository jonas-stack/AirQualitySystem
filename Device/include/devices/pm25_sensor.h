#ifndef PM25_SENSOR_H
#define PM25_SENSOR_H

#include <Arduino.h>
#include <PMS.h>
#include <HardwareSerial.h>

bool setupPM25Sensor();
float readPM25Value();
void updatePM25SensorValues();

#endif