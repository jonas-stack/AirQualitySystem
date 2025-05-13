#include "JsonSerializer.h"

const size_t JSON_CAPACITY = 256; //represent the memory alocated to JSON documents

String JsonSerializer::serializeSensorData(float temperature, float humidity, float gas, float particles, const char* device_id) {
    DynamicJsonDocument doc(256);
    
    // Sensor data
    doc["temperature"] = temperature;
    doc["humidity"] = humidity;
    doc["air_quality"] = gas;
    doc["pm25"] = particles;
    doc["device_id"] = device_id;
    
    // Get formatted time instead of timestamp
    doc["timestamp"] = getFormattedTime();
    
    String output;
    serializeJson(doc, output);
    return output;
}

String JsonSerializer::getFormattedTime() {
    time_t now = time(nullptr);
    struct tm timeinfo;
    gmtime_r(&now, &timeinfo);
    char timeString[30];
    strftime(timeString, sizeof(timeString), "%Y-%m-%d %H:%M:%S", &timeinfo);
    return String(timeString);
}

String JsonSerializer::serializeStatusMessage(const char* status, const char* device_id) {
    // Create JSON document for status messages
    StaticJsonDocument<JSON_CAPACITY> doc;
    
    doc["status"] = status;
    doc["device_id"] = device_id;
    doc["timestamp"] = getFormattedTime();  // Use formatted time instead of Unix timestamp
    
    // Serialize JSON to string
    String jsonString;
    serializeJson(doc, jsonString);
    
    return jsonString;
}