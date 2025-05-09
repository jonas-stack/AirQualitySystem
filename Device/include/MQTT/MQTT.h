#ifndef MQTT_H
#define MQTT_H

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
bool clearRetainedMessage(const char* topic);

extern WiFiClientSecure espClient;
extern PubSubClient* client;

#endif // MQTT_H