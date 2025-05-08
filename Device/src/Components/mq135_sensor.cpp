#include "devices/mq135_sensor.h"

// Define MQ135 sensor parameters
#define Board "ESP-32"
#define Pin 36 // (ADC1 input-only)
#define Type "MQ-135"
#define Voltage_Resolution 3.3  // ESP32 ADC voltage
#define ADC_Bit_Resolution 12   // ESP32 ADC resolution
#define RatioMQ135CleanAir 3.6  // RS/R0 at clean air for MQ135
#define ATMOSPHERIC_CO2 400.0 // Base CO2 level in atmosphere (for calculation reference)

MQUnifiedsensor MQ135(Board, Voltage_Resolution, ADC_Bit_Resolution, Pin, Type);

float rawPPM = 0;
float correctedPPM = 0;
int rawADC = 0;
float voltage = 0;

bool setupMQ135Sensor() {
    Serial.println("Initializing MQ135 sensor...");

    // With this more robust approach:
    pinMode(Pin, INPUT);
    int testValue = 0;
    
    // Use the MQ135 library's own methods instead of direct analogRead
    MQ135.init(); 
    MQ135.update();
    testValue = MQ135.getVoltage() > 0 ? 1 : 0;

    // A simple check - if reading is 0 or max value, something is probably wrong
    if (testValue == 0 || testValue == 4095) {
        Serial.println("MQ135 sensor initialization failed!");
        return false;
    }
    
    // Configure for CO2 detection
    MQ135.setRegressionMethod(1);
    MQ135.setA(110.47); MQ135.setB(-2.862); // Parameters for CO2
    
    MQ135.init();
    
    // Important calibration step
    Serial.print("Calibrating MQ135 sensor");
    float calcR0 = 0;
    for(int i = 1; i<=10; i++) {
      MQ135.update();
      calcR0 += MQ135.calibrate(RatioMQ135CleanAir);
      Serial.print(".");
      delay(100);
    }
    Serial.println();

    MQ135.setR0(calcR0/10);
    Serial.print("Calibration value R0: ");
    Serial.println(calcR0/10);
    Serial.println("MQ135 sensor initialized successfully for CO2 detection.");
    
    // Update global variables with initial values
    updateSensorValues();
    
    return true;
}

// Function to update all global sensor values
void updateSensorValues() {
    MQ135.update();
    rawPPM = MQ135.readSensor();  
    correctedPPM = ATMOSPHERIC_CO2 + (rawPPM * 100);
    
    if (correctedPPM < ATMOSPHERIC_CO2) 
    {
      correctedPPM = ATMOSPHERIC_CO2;
    }
    
    voltage = MQ135.getVoltage(); 
}

float readMQ135Sensor() {
    updateSensorValues();  
    return correctedPPM;
}

void printMQ135SensorData() {
    updateSensorValues();  // Update all global values
    
    Serial.print("Voltage: ");
    Serial.println(voltage, 2);  
    Serial.print("Raw sensor value: ");
    Serial.println(rawPPM, 4);
    Serial.print("CO2 PPM: ");
    Serial.println(correctedPPM, 2);
    Serial.println("---------------------");
}