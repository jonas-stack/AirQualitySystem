#include "pm25_sensor.h"


#define PMS_SERIAL Serial2
#define PMS_RX_PIN 16  // GPIO pin for RX (ESP32 receives data from sensor here)


PMS pms(PMS_SERIAL);
PMS::DATA data;

bool setupPM25Sensor()
{
  Serial.println("Initializing PM2.5 sensor...");
  
  // Initialize hardware serial with only RX pin
  PMS_SERIAL.begin(9600, SERIAL_8N1, PMS_RX_PIN, -1);  // -1 means no TX pin
  
  // Give time for the sensor to start up
  delay(1000);
  
  Serial.println("PM2.5 sensor initialized successfully.");
  return true;
  delay(5000);
}

void printPM25SensorData()
{
  // We can't request a read without TX, so we just wait for data
  if (pms.readUntil(data, 1000)) {  // 1000ms timeout
    Serial.print("PM 1.0 (ug/m3): ");
    Serial.println(data.PM_AE_UG_1_0);

    Serial.print("PM 2.5 (ug/m3): ");
    Serial.println(data.PM_AE_UG_2_5);

    Serial.print("PM 10.0 (ug/m3): ");
    Serial.println(data.PM_AE_UG_10_0);

    Serial.println("---------------------");
  } else {
    Serial.println("Failed to read from PM2.5 sensor");
  }
}

float readPM25Value() 
{
  // We can't request a read without TX, so we just wait for data
  if (pms.readUntil(data, 1000)) {
    return data.PM_AE_UG_2_5;
  }
  
  return -1.0; // Error value
}