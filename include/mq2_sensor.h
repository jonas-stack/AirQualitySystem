#ifndef MQ2_SENSOR_H
#define MQ2_SENSOR_H

#include <Arduino.h>
#include <MQUnifiedsensor.h>

void setupMQ2Sensor();
float readMQ2Sensor();
float readLPG();
float readCO();
float readSmoke();

#endif