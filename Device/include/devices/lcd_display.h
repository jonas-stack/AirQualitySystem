#ifndef LCD_DISPLAY_H
#define LCD_DISPLAY_H

#include <Arduino.h>
#include <LiquidCrystal_I2C.h>

/**
 * @brief Initializes the LCD display.
 * @return true if the display is found and initialized, false otherwise.
 */
bool setupLCDDisplay();

/**
 * @brief Displays two lines of text on the LCD.
 * @param line1 Text for the first line.
 * @param line2 Text for the second line.
 */
void displayMessage(const char* line1, const char* line2);

/**
 * @brief Cycles through and displays temperature, humidity, CO2, and PM2.5 values.
 * Each call shows the next value in sequence.
 * @param temperature Current air temperature (°C).
 * @param humidity Current air humidity (%).
 * @param gas Current CO2 level (PPM).
 * @param particles Current PM2.5 concentration (μg/m³).
 */
void updateLCDDisplay(float temperature, float humidity, float gas, float particles);

#endif // LCD_DISPLAY_H