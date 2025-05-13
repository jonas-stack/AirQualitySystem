#include "TimeManager.h"

TimeManager::TimeManager() {
}

bool TimeManager::syncNTP() {
    Serial.print("Waiting for NTP time sync: ");
    configTime(0, 0, "pool.ntp.org", "time.nist.gov");
    
    time_t now = time(nullptr);
    unsigned long startTime = millis();
    
    while (now < 8 * 3600 * 2) {
        delay(100);
        Serial.print(".");
        now = time(nullptr);
        
        // Timeout after 30 seconds
        if (millis() - startTime > 30000) {
            Serial.println("\nFailed to sync with NTP");
            return false;
        }
    }
    Serial.println();

    struct tm timeinfo;
    gmtime_r(&now, &timeinfo);
    Serial.print("Current UTC time: ");
    Serial.println(asctime(&timeinfo));
    
    return true;
}

String TimeManager::getCurrentTime() {
    time_t now = time(nullptr);
    struct tm timeinfo;
    gmtime_r(&now, &timeinfo);
    char timeString[30];
    strftime(timeString, sizeof(timeString), "%Y-%m-%d %H:%M:%S", &timeinfo);
    return String(timeString);
}

time_t TimeManager::getNow() {
    return time(nullptr);
}