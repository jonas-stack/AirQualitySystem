#ifndef JSON_SERIALIZER_H
#define JSON_SERIALIZER_H

#include <Arduino.h>
#include <ArduinoJson.h>
#include <time.h>

/**
 * @brief Serializes sensor and status data to JSON format for MQTT publishing.
 */
class JsonSerializer {
public:
    JsonSerializer();

    /**
     * @brief Serializes sensor readings and device ID into a JSON string.
     * @param temperature Temperature value.
     * @param humidity Humidity value.
     * @param gas CO2/gas value.
     * @param particles PM2.5 value.
     * @param device_id Device identifier string.
     * @return JSON string with sensor data and timestamp.
     */
    static String serializeSensorData(float temperature, float humidity, float gas, float particles, const char* device_id);

    /**
     * @brief Serializes device status and ID into a JSON string.
     * @param status Device status string ("online", "offline", etc.).
     * @param device_id Device identifier string.
     * @return JSON string with status and device ID.
     */
    static String serializeStatusMessage(const char* status, const char* device_id, int updateinterval);
};

#endif // JSON_SERIALIZER_H