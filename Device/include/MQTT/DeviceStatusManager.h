#pragma once

#include <Arduino.h>
#include "ConnectionManager.h"
#include "TimeManager.h"

class DeviceStatusManager {
private:
    ConnectionManager* _connectionManager;
    TimeManager* _timeManager;
    const char* _deviceId;
    
    unsigned long _lastStatusUpdate;
    unsigned long _statusUpdateInterval;
    String _connectionTime;
    const char* _statusTopic;
    
public:
    DeviceStatusManager(ConnectionManager* connectionManager, 
                        TimeManager* timeManager,
                        const char* deviceId,
                        const char* statusTopic = "airquality/status",
                        unsigned long updateInterval = 5 * 60 * 1000);
    
    // Prepare offline message for LWT
    String prepareOfflineMessage();
    
    // Prepare and send online message
    bool publishOnlineStatus();
    
    // Update device status periodically
    bool updateDeviceStatus();
    
    // Check if status update is needed
    void checkStatusUpdate();
    
    // Reset connection time
    void resetConnectionTime();
    
    // Set connection time to current time
    void updateConnectionTime();
    
    // Clear retained status message
    bool clearRetainedStatus();
};