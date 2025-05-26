#ifndef MQ135_SENSOR_H
#define MQ135_SENSOR_H

#include <Arduino.h>
#include <MQUnifiedsensor.h>
#include <WiFi.h>

bool setupMQ135Sensor();
float readMQ135Sensor();
void updateSensorValues();

#endif