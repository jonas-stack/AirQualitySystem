#ifndef LCD_DISPLAY_H
#define LCD_DISPLAY_H

#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

void setupLCD();
void updateLCDDisplay(float temperature, float humidity, float gas, float particles);
void displayMessage(const char* line1, const char* line2);
void clearLCDDisplay();
bool setupLCDDisplay();

#endif