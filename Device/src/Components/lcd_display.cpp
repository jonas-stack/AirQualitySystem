#include "devices/lcd_display.h"


LiquidCrystal_I2C lcd(0x27, 16, 2);
int displayState = 0;

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
  delay(5000);
}

void updateLCDDisplay(float temperature, float humidity, float gas, float particles) {
  lcd.clear();
  
  switch (displayState) {
    case 0:
      // Show temperature
      lcd.setCursor(0, 0);
      lcd.print("Air Temperature: ");
      lcd.setCursor(0, 1);
      lcd.print(temperature);
      lcd.print(" C ");
      break;
    
    case 1:
      // Show humidity
      lcd.setCursor(0, 0);
      lcd.print("Air Humidity: ");
      lcd.setCursor(0, 1);
      lcd.print(humidity);
      lcd.print(" % ");
      break;
    
    case 2:
      // Show CO2 level
      lcd.setCursor(0, 0);
      lcd.print("CO2 Level: "); 
      lcd.setCursor(0, 1);
      lcd.print(gas);
      lcd.print(" PPM ");
      break;

    case 3:
      // Show PM2.5 particles
      lcd.setCursor(0, 0);
      lcd.print("PM2.5 Particles: ");
      lcd.setCursor(0, 1);
      lcd.print(particles);
      lcd.print(" ug/m3 ");
      break;
  }
  
  // Move to next state
  displayState = (displayState + 1) % 4; //increase loop sice for each case added
}

