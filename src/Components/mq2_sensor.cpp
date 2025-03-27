#include "mq2_sensor.h"

// Define MQ2 sensor parameters
#define Board "ESP-32"  // Changed to ESP-32 from Arduino UNO
#define Pin 14 // Analog pin
#define Type "MQ-2"
#define Voltage_Resolution 3.3  // ESP32 typically uses 3.3V reference
#define ADC_Bit_Resolution 12   // ESP32 has 12-bit ADC (0-4095)
#define RatioMQ2CleanAir 9.83


MQUnifiedsensor MQ2(Board, Voltage_Resolution, ADC_Bit_Resolution, Pin, Type);

void setupMQ2Sensor() {
    
    // Set math model to calculate gas concentration
    MQ2.setRegressionMethod(1); // _PPM = a*ratio^b
    MQ2.setA(574.25); MQ2.setB(-2.222); // Configure equation for LPG
    
    MQ2.init();
    
    // Important calibration step
    Serial.println("Calibrating MQ-2 sensor...");
    float calcR0 = 0;
    for(int i = 1; i<=10; i++) {
        MQ2.update();
        calcR0 += MQ2.calibrate(RatioMQ2CleanAir);
        Serial.print(".");
        delay(1000);
    }
    MQ2.setR0(calcR0/10);
    Serial.println("\nCalibration completed!");
    Serial.println("MQ2 sensor initialized successfully");
}

float readMQ2Sensor() {

    MQ2.update(); // Update data
    return MQ2.readSensor();
}

void printMQ2SensorData() {
    int rawADC = analogRead(Pin);
    float voltage = (rawADC * 3.3) / 4095.0;  // Convert to voltage
    float readSensor = readMQ2Sensor();
    
    Serial.print("Raw ADC value: ");
    Serial.println(rawADC);
    Serial.print("Voltage: ");
    Serial.println(voltage);
    Serial.print("MQ2 Sensor PPM: ");
    Serial.println(readSensor);
    Serial.println("---------------------");
}