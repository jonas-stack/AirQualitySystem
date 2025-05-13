#ifndef JSON_SERIALIZER_H
#define JSON_SERIALIZER_H

#include <Arduino.h>
#include <ArduinoJson.h>
#include <time.h>

class JsonSerializer {
public:
    JsonSerializer();
    
    // Updated method with a formatted timestamp
    static String serializeSensorData(float temperature, float humidity, float gas, float particles, const char* device_id);
    
    // Helper method to get formatted time
    static String getFormattedTime();
    
    static String serializeStatusMessage(const char* status, const char* device_id);
};

#endif