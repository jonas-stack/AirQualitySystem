#include "TimeManager.h"

TimeManager::TimeManager() {}

bool TimeManager::syncNTP() {
    configTime(0, 0, "pool.ntp.org", "time.nist.gov");
    time_t now = time(nullptr);
    unsigned long startTime = millis();
    
    while (now < 8 * 3600 * 2) {
        delay(100);
        now = time(nullptr);
        if (millis() - startTime > 30000) {
            return false;
        }
    }
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