#include "JsonSerializer.h"

JsonSerializer::JsonSerializer() {
    // Constructor (empty for now as we're using static methods)
}

String JsonSerializer::serializeSensorData(float temperature, float humidity, float gas, float particles, const char* device_id) {
    // Create JSON document for sensor data
    StaticJsonDocument<JSON_CAPACITY> doc;
    
    doc["temperature"] = temperature;
    doc["humidity"] = humidity;
    doc["air_quality"] = gas;
    doc["pm25"] = particles;
    doc["device_id"] = device_id;
    doc["timestamp"] = time(nullptr);
    
    // Serialize JSON to string
    String jsonString;
    serializeJson(doc, jsonString);
    
    return jsonString;
}

String JsonSerializer::serializeStatusMessage(const char* status, const char* device_id) {
    // Create JSON document for status messages
    StaticJsonDocument<JSON_CAPACITY> doc;
    
    doc["status"] = status;
    doc["device_id"] = device_id;
    doc["timestamp"] = time(nullptr);
    
    // Serialize JSON to string
    String jsonString;
    serializeJson(doc, jsonString);
    
    return jsonString;
}