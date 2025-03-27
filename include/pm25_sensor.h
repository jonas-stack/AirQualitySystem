#ifndef PM25_SENSOR_H
#define PM25_SENSOR_H

#include <Arduino.h>
#include <PMS.h>

void setupPM25Sensor();
int readPM25Value();
int readPM10Value();
int readPM1Value();

#endif