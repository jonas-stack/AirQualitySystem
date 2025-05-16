#ifndef TIME_MANAGER_H
#define TIME_MANAGER_H

#include <Arduino.h>
#include <time.h>

class TimeManager {
public:
    TimeManager();
    bool syncNTP();
    String getCurrentTime();
    time_t getUnixTime();
};

#endif