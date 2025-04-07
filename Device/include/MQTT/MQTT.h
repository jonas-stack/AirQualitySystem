#ifndef MQTT_CLIENT_H
#define MQTT_CLIENT_H

#include <Arduino.h>
#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <PubSubClient.h>
#include <time.h>
#include <SPIFFS.h>
#include <ArduinoJson.h>

// Function declarations
bool setupMQTTClient();
void loopMQTTClient();
bool publishSensorData(float temperature, float humidity, float gas, float particles);

// External declaration of any variables that might be needed in main.cpp
extern WiFiClientSecure espClient;
extern PubSubClient* client;

#endif // MQTT_CLIENT_H