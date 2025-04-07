#include "devices/pm25_sensor.h"


#define PMS_SERIAL Serial2
#define PMS_RX_PIN 16  // GPIO pin for RX (ESP32 receives data from sensor here)


PMS pms(PMS_SERIAL);
PMS::DATA data;

int PM25Value = 0; 
int PM10Value = 0; 
int PM1Value = 0;  

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

void updatePM25SensorValues()
{
    pms.readUntil(data, 1000);  // 1000ms timeout
    PM25Value = data.PM_AE_UG_2_5;  // PM2.5 value in micrograms per cubic meter
    PM10Value = data.PM_AE_UG_10_0; // PM10 value in micrograms per cubic meter
    PM1Value = data.PM_AE_UG_1_0;   // PM1 value in micrograms per cubic meter
}

void printPM25SensorData()
{
    // Update the global values first
    updatePM25SensorValues();
    
    // If we have valid data (non-negative values), print it
    if (PM1Value >= 0) {
        Serial.print("PM 1.0 (ug/m3): ");
        Serial.println(PM1Value);
        
        Serial.print("PM 2.5 (ug/m3): ");
        Serial.println(PM25Value);
        
        Serial.print("PM 10.0 (ug/m3): ");
        Serial.println(PM10Value);
        
        Serial.println("---------------------");
    } else {
        Serial.println("Failed to read from PM2.5 sensor");
    }
}

float readPM25Value() 
{
    // Update all sensor values
    updatePM25SensorValues();
    
    // Return the global PM2.5 value
    return PM25Value;
}