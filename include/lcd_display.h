#ifndef LCD_DISPLAY_H
#define LCD_DISPLAY_H

#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

void setupLCD();
void updateLCD(float temperature, float humidity, float gasValue, int pm25Value);
void displayMessage(const char* line1, const char* line2);

#endif