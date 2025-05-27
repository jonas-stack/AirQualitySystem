#include "devices/lcd_display.h"

// LCD object for 16x2 I2C display at address 0x27.
LiquidCrystal_I2C lcd(0x27, 16, 2);

// Tracks which sensor value is currently displayed.
int displayState = 0;

/**
 * Initializes the LCD display.
 * @return true if the display is found and initialized, false otherwise.
 */
bool setupLCDDisplay() {
    Serial.println("Initializing LCD display...");
    
    Wire.beginTransmission(0x27);
    bool deviceFound = (Wire.endTransmission() == 0);
    
    if (!deviceFound) {
        Serial.println("LCD display initialization failed!");
        return false;
    }
    
    lcd.init();
    lcd.backlight();
    Serial.println("LCD display initialized successfully.");
    return true;
}

/**
 * Displays two lines of text on the LCD.
 * @param line1 Text for the first line.
 * @param line2 Text for the second line.
 */
void displayMessage(const char* line1, const char* line2) {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(line1);
    lcd.setCursor(0, 1);
    lcd.print(line2);
}

/**
 * Cycles through and displays temperature, humidity, CO2, and PM2.5 values.
 * Each call shows the next value in sequence.
 * @param temperature Current air temperature (°C).
 * @param humidity Current air humidity (%).
 * @param gas Current CO2 level (PPM).
 * @param particles Current PM2.5 concentration (μg/m³).
 */
void updateLCDDisplay(float temperature, float humidity, float gas, float particles) {
    lcd.clear();
    
    switch (displayState) {
        case 0:
            // Show temperature.
            lcd.setCursor(0, 0);
            lcd.print("Air Temperature: ");
            lcd.setCursor(0, 1);
            lcd.print(temperature);
            lcd.print(" C ");
            break;
        
        case 1:
            // Show humidity.
            lcd.setCursor(0, 0);
            lcd.print("Air Humidity: ");
            lcd.setCursor(0, 1);
            lcd.print(humidity);
            lcd.print(" % ");
            break;
        
        case 2:
            // Show CO2 level.
            lcd.setCursor(0, 0);
            lcd.print("CO2 Level: "); 
            lcd.setCursor(0, 1);
            lcd.print(gas);
            lcd.print(" PPM ");
            break;

        case 3:
            // Show PM2.5 particles.
            lcd.setCursor(0, 0);
            lcd.print("PM2.5 Particles: ");
            lcd.setCursor(0, 1);
            lcd.print(particles);
            lcd.print(" ug/m3 ");
            break;
    }
    
    // Advance to the next display state for the next update.
    displayState = (displayState + 1) % 4; // Increase loop size if more cases are added.
}