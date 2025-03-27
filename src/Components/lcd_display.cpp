#include "lcd_display.h"
#include <LiquidCrystal_I2C.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);

void setupLCDDisplay() {
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Temp: ");
  lcd.setCursor(0, 1);
  lcd.print("Fugt: ");
}

void updateLCDDisplay(float temperature, float humidity) {
    lcd.setCursor(10, 0);
    lcd.print(temperature);
    lcd.setCursor(10, 1);
    lcd.print(humidity);
}

void clearLCDDisplay() {
  lcd.clear();
}

