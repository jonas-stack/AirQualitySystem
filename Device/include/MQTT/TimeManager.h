#pragma once

#include <Arduino.h>
#include <time.h>

class TimeManager {
public:
    TimeManager();
    bool syncNTP();
    String getCurrentTime();
    time_t getNow();
};