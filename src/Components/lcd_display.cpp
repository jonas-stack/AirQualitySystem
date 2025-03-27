#include "lcd_display.h"


LiquidCrystal_I2C lcd(0x27, 16, 2);
int displayState = 0;

void setupLCDDisplay() {
  lcd.init();
  lcd.backlight();
}

void updateLCDDisplay(float temperature, float humidity, float gas) {
  lcd.clear();
  
  switch (displayState) {
    case 0:
      // Show temperature
      lcd.setCursor(0, 0);
      lcd.print("Temperature:");
      lcd.setCursor(0, 1);
      lcd.print(temperature);
      lcd.print(" C");
      break;
    
    case 1:
      // Show humidity
      lcd.setCursor(0, 0);
      lcd.print("Humidity:");
      lcd.setCursor(0, 1);
      lcd.print(humidity);
      lcd.print(" %");
      break;
    
    case 2:
      // Show gas
      lcd.setCursor(0, 0);
      lcd.print("Gas level:");
      lcd.setCursor(0, 1);
      lcd.print(gas);
      lcd.print(" PPM");
      break;
  }
  
  // Move to next state
  displayState = (displayState + 1) % 3;
}

