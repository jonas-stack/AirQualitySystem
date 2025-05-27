#include "devices/mq135_sensor.h"

// MQ135 sensor configuration parameters.
#define Board "ESP-32"
#define Pin 36 // ADC1 input-only pin for MQ135
#define Type "MQ-135"
#define Voltage_Resolution 3.3      // ESP32 ADC voltage reference
#define ADC_Bit_Resolution 12       // ESP32 ADC resolution
#define RatioMQ135CleanAir 3.6      // RS/R0 ratio in clean air for MQ135
#define ATMOSPHERIC_CO2 400.0       // Reference CO2 level in clean air (ppm)

// MQ135 sensor object.
MQUnifiedsensor MQ135(Board, Voltage_Resolution, ADC_Bit_Resolution, Pin, Type);

// Global variables to store sensor readings.
static float rawPPM = 0;
static float correctedPPM = 0;
static int rawADC = 0;
static float voltage = 0;

/**
 * Initializes and calibrates the MQ135 sensor for CO2 detection.
 * @return true if initialization and calibration succeed, false otherwise.
 */
bool setupMQ135Sensor() {
    Serial.println("Initializing MQ135 sensor...");

    pinMode(Pin, INPUT);
    int testValue = 0;
    MQ135.init();
    MQ135.update();
    testValue = MQ135.getVoltage() > 0 ? 1 : 0;

    // Check for sensor connection issues.
    if (testValue == 0 || testValue == 4095) {
        Serial.println("MQ135 sensor initialization failed!");
        return false;
    }

    // Configure regression method and parameters for CO2 detection.
    MQ135.setRegressionMethod(1);
    MQ135.setA(110.47);
    MQ135.setB(-2.862);

    MQ135.init();

    // Calibrate sensor in clean air to determine R0 value.
    Serial.print("Calibrating MQ135 sensor");
    float calcR0 = 0;
    for (int i = 1; i <= 10; i++) {
        MQ135.update();
        calcR0 += MQ135.calibrate(RatioMQ135CleanAir);
        Serial.print(".");
        delay(100);
    }
    Serial.println();

    MQ135.setR0(calcR0 / 10);
    Serial.print("Calibration value R0: ");
    Serial.println(calcR0 / 10);
    Serial.println("MQ135 sensor initialized successfully for CO2 detection.");

    updateSensorValues();

    return true;
}

/**
 * Updates all global sensor values from the MQ135 sensor.
 */
void updateSensorValues() {
    MQ135.update();
    rawPPM = MQ135.readSensor();
    correctedPPM = ATMOSPHERIC_CO2 + (rawPPM * 100);

    // Ensure CO2 value does not fall below atmospheric baseline.
    if (correctedPPM < ATMOSPHERIC_CO2) {
        correctedPPM = ATMOSPHERIC_CO2;
    }

    voltage = MQ135.getVoltage();
}

/**
 * Reads and returns the latest corrected CO2 value from the MQ135 sensor.
 * @return CO2 concentration in ppm.
 */
float readMQ135Sensor() {
    updateSensorValues();
    return correctedPPM;
}