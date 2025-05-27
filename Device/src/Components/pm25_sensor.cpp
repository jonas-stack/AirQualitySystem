#include "devices/pm25_sensor.h"

// Hardware serial port and pin configuration for the PM2.5 sensor.
#define PMS_SERIAL Serial2
#define PMS_RX_PIN 16  // GPIO pin for RX (ESP32 receives data from sensor here)

// PMS sensor object and data structure.
PMS pms(PMS_SERIAL);
PMS::DATA data;

// Global variables to store the latest PM sensor readings.
static int PM25Value = 0; 
static int PM10Value = 0; 
static int PM1Value = 0;  

/**
 * Initializes the PM2.5 sensor hardware and serial communication.
 * @return true if initialization succeeds.
 */
bool setupPM25Sensor()
{
    Serial.println("Initializing PM2.5 sensor...");
    
    // Initialize hardware serial for the sensor (RX only).
    PMS_SERIAL.begin(9600, SERIAL_8N1, PMS_RX_PIN, -1);  // -1 means no TX pin
    
    // Allow sensor time to start up.
    delay(1000);
    
    Serial.println("PM2.5 sensor initialized successfully.");
    return true;
}

/**
 * Reads data from the PM2.5 sensor and updates global values.
 */
void updatePM25SensorValues()
{
    // Read sensor data with a 1000ms timeout.
    pms.readUntil(data, 1000);
    PM25Value = data.PM_AE_UG_2_5;   // PM2.5 value (μg/m³)
    PM10Value = data.PM_AE_UG_10_0;  // PM10 value (μg/m³)
    PM1Value = data.PM_AE_UG_1_0;    // PM1.0 value (μg/m³)
}

/**
 * Gets the latest PM2.5 value from the sensor.
 * @return PM2.5 concentration in micrograms per cubic meter.
 */
float readPM25Value() 
{
    updatePM25SensorValues();
    return PM25Value;
}