#include "WiFi/ResetDetector.h"

ResetDetector::ResetDetector(int timeout, int address) :
    _timeout(timeout),
    _address(address) {
    _drd = new DoubleResetDetector(timeout, address);
}

ResetDetector::~ResetDetector() {
    if (_drd) delete _drd;
}

bool ResetDetector::detectDoubleReset() {
    return _drd->detectDoubleReset();
}

void ResetDetector::loop() {
    if (_drd) _drd->loop();
}