#ifndef MQ2_SENSOR_H
#define MQ2_SENSOR_H

#include <Arduino.h>
#include <MQUnifiedsensor.h>

bool setupMQ2Sensor();
float readMQ2Sensor();
float readLPG();
float readCO();
float readSmoke();
void printMQ2SensorData();

#endif