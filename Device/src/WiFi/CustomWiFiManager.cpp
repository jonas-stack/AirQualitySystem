#include "WiFi/CustomWiFiManager.h"
#include "MQTT/config.h"
#include "devices/lcd_display.h"

CustomWiFiManager::CustomWiFiManager(const char* ssid, const char* password, 
                                   const char* apName, const char* apPassword,
                                   int drdTimeout, int drdAddress,
                                   int configButtonPin) :
    _configMode(false),
    _configButtonPin(configButtonPin),
    _lastButtonState(HIGH),
    _lastDebounceTime(0) {
    
    // Initialize components
    _connection = new WiFiConnection(ssid, password);
    _portalManager = new ConfigPortalManager(apName, apPassword);
    _resetDetector = new ResetDetector(drdTimeout, drdAddress);
    
    // Initialize config button if specified
    if (_configButtonPin >= 0) {
        pinMode(_configButtonPin, INPUT_PULLUP);
        Serial.print("WiFi config button initialized on GPIO ");
        Serial.println(_configButtonPin);
    }
}

CustomWiFiManager::~CustomWiFiManager() {
    if (_connection) delete _connection;
    if (_portalManager) delete _portalManager;
    if (_resetDetector) delete _resetDetector;
}

bool CustomWiFiManager::connect() {
    displayMessage("WiFi", "Connecting...");
    
    // Check for double reset
    if (_resetDetector->detectDoubleReset()) {
        Serial.println("Double reset detected - starting config portal");
        
        // Visual indication
        blinkLED(10, 100);
        
        // Start config portal
        startConfigPortal();
        return true;
    }
    
    // Try autoConnect first
    if (_portalManager->autoConnect()) {
        displayMessage("WiFi Connected", "Using saved");
        delay(1000);
        
        displayMessage("IP:", _connection->getIp().c_str());
        delay(1000);
        
        return true;
    }
    
    // If autoConnect fails, try direct connection
    return _connection->connect();
}

bool CustomWiFiManager::isConnected() {
    return _connection->isConnected();
}

void CustomWiFiManager::disconnect() {
    _connection->disconnect();
}

void CustomWiFiManager::resetSettings() {
    _portalManager->resetSettings();
    ESP.restart();
}

void CustomWiFiManager::startConfigPortal() {
    Serial.println("Starting config portal");
    displayMessage("WiFi Setup Mode", "Connect to AP:");
    delay(1000);
    displayMessage(WIFI_AP_NAME, WIFI_AP_PASSWORD);
    
    _configMode = true;
    
    if (!_portalManager->startPortal()) {
        Serial.println("Config portal failed");
        displayMessage("WiFi Setup", "Failed/Timeout");
        delay(2000);
        ESP.restart();
    } else {
        Serial.println("WiFi connected via portal");
        displayMessage("WiFi Connected", "Via Portal");
        delay(1000);
        
        displayMessage("IP:", _connection->getIp().c_str());
        delay(1000);
    }
}

void CustomWiFiManager::checkConfigButton() {
    if (_configButtonPin < 0) return;
    
    bool reading = digitalRead(_configButtonPin);
    
    // Button press detection with debouncing
    if (reading != _lastButtonState) {
        _lastDebounceTime = millis();
    }
    
    // Check if button state is stable
    if ((millis() - _lastDebounceTime) > 50) {
        // Detect button press (transition from HIGH to LOW)
        if (reading == LOW && _lastButtonState == HIGH) {
            handleButtonPress();
        }
    }
    
    _lastButtonState = reading;
}

void CustomWiFiManager::handleButtonPress() {
    Serial.println("Config button pressed!");
    
    // Track how long button is held
    unsigned long pressStartTime = millis();
    bool longPressDetected = false;
    
    // Wait for button release or long press detection
    while (digitalRead(_configButtonPin) == LOW) {
        // Check for long press (3 seconds)
        if (millis() - pressStartTime > 3000 && !longPressDetected) {
            Serial.println("Long press detected - resetting WiFi");
            displayMessage("WiFi Reset", "In Progress...");
            
            // Provide visual feedback
            blinkLED(5, 100);
            
            // Reset WiFi settings
            resetSettings();
            longPressDetected = true;
        }
        delay(10);
    }
    
    // If it was a short press and not a long press
    if (!longPressDetected) {
        Serial.println("Short press - starting portal");
        startConfigPortal();
    }
}

void CustomWiFiManager::blinkLED(int times, int delayMs) {
    for (int i = 0; i < times; i++) {
        digitalWrite(2, HIGH);
        delay(delayMs);
        digitalWrite(2, LOW);
        delay(delayMs);
    }
}

void CustomWiFiManager::loop() {
    _resetDetector->loop();
    checkConfigButton();
}