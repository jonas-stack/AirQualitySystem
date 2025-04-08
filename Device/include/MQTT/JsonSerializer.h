#ifndef JSON_SERIALIZER_H
#define JSON_SERIALIZER_H

#include <Arduino.h>
#include <ArduinoJson.h>
#include <time.h>

class JsonSerializer {
public:
    JsonSerializer();

    // Serialize sensor data to JSON string
    static String serializeSensorData(float temperature, float humidity, float gas, float particles, const char* device_id);
    
    // Serialize status message to JSON string
    static String serializeStatusMessage(const char* status, const char* device_id);

private:
    static const size_t JSON_CAPACITY = 256;
};

#endif // JSON_SERIALIZER_H