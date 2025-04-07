#include "mq2_sensor.h"

// Define MQ2 sensor parameters
#define Board "ESP-32"  // Changed to ESP-32 from Arduino UNO
#define Pin 14 // Analog pin
#define Type "MQ-2"
#define Voltage_Resolution 3.3  // ESP32 typically uses 3.3V reference
#define ADC_Bit_Resolution 12   // ESP32 has 12-bit ADC (0-4095)
#define RatioMQ2CleanAir 9.83


MQUnifiedsensor MQ2(Board, Voltage_Resolution, ADC_Bit_Resolution, Pin, Type);

bool setupMQ2Sensor() {
  Serial.println("Initializing MQ2 sensor...");
  
  // Test if the analog pin is working
  int testValue = analogRead(Pin);
  
  // A simple check - if reading is 0 or max value, something is probably wrong
  if (testValue == 0 || testValue == 4095) {
    Serial.println("MQ2 sensor initialization failed!");
    return false;
  }
  
  // Continue with your existing setup code
  MQ2.setRegressionMethod(1);
  MQ2.setA(36974); MQ2.setB(-3.109);
  
  MQ2.init();
  
  // Important calibration step
  Serial.print("Calibrating MQ2 sensor");
  float calcR0 = 0;
  for(int i = 1; i<=10; i++) {
    MQ2.update();
    calcR0 += MQ2.calibrate(RatioMQ2CleanAir);
    Serial.print(".");
  }
  Serial.println();

  MQ2.setR0(calcR0/10);
  Serial.println("MQ2 sensor initialized successfully.");
  return true;
  delay(5000);
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