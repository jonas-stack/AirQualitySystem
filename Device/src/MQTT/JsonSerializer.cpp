#include "JsonSerializer.h"

JsonSerializer::JsonSerializer() {}

String JsonSerializer::serializeSensorData(float temperature, float humidity, float gas, float particles, const char* device_id) {
    DynamicJsonDocument doc(256);
    doc["temperature"] = temperature;
    doc["humidity"] = humidity;
    doc["air_quality"] = gas;
    doc["pm25"] = particles;
    doc["device_id"] = device_id;
    doc["timestamp"] = time(nullptr); 
    String output;
    serializeJson(doc, output);
    return output;
}

String JsonSerializer::serializeStatusMessage(const char* status, const char* device_id, int update_interval) {
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = device_id;
    doc["IsConnected"] = (strcmp(status, "online") == 0);
    doc["LastSeen"] = time(nullptr);
    doc["Updateinterval"] = update_interval;
    String jsonString;
    serializeJson(doc, jsonString);
    return jsonString;
}