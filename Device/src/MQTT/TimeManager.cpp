#include "TimeManager.h"

TimeManager::TimeManager() {}

bool TimeManager::syncNTP() {
    // Set timezone to UTC+2 (7200 seconds)
    configTime(7200, 0, "pool.ntp.org");
    
    // Wait up to 30 seconds for time to be set
    unsigned long startTime = millis();
    while (time(nullptr) < 1600000000) {  // Reasonable timestamp after 2020
        delay(100);
        if (millis() - startTime > 30000) {
            return false;
        }
    }
    return true;
}

String TimeManager::getCurrentTime() {
    struct tm timeinfo;
    if (!getLocalTime(&timeinfo)) {
        return "Failed to get time";
    }
    
    char timeString[30];
    strftime(timeString, sizeof(timeString), "%Y-%m-%d %H:%M:%S", &timeinfo);
    return String(timeString);
}