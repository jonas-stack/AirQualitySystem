#ifndef RESET_DETECTOR_H
#define RESET_DETECTOR_H

#include <ESP_DoubleResetDetector.h>

class ResetDetector {
private:
    DoubleResetDetector* _drd;
    int _timeout;
    int _address;
    
public:
    ResetDetector(int timeout, int address);
    ~ResetDetector();
    
    bool detectDoubleReset();
    void loop();
};

#endif // RESET_DETECTOR_H