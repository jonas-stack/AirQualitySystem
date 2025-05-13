#include "DeviceStatusManager.h"
#include <ArduinoJson.h>

DeviceStatusManager::DeviceStatusManager(ConnectionManager* connectionManager, 
                                       TimeManager* timeManager,
                                       const char* deviceId,
                                       const char* statusTopic,
                                       unsigned long updateInterval) :
    _connectionManager(connectionManager),
    _timeManager(timeManager),
    _deviceId(deviceId),
    _lastStatusUpdate(0),
    _statusUpdateInterval(updateInterval),
    _statusTopic(statusTopic) {
}

String DeviceStatusManager::prepareOfflineMessage() {
    // Get current time as the last seen time
    String timeString = _timeManager->getCurrentTime();
    
    // Store connection time if not set
    if (_connectionTime.isEmpty()) {
        _connectionTime = timeString;
    }
    
    // Create offline message with both timestamps
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = _deviceId;
    doc["IsConnected"] = false;
    doc["LastSeen"] = timeString;
    doc["ConnectedSince"] = _connectionTime;
    
    String message;
    serializeJson(doc, message);
    return message;
}

bool DeviceStatusManager::publishOnlineStatus() {
    // Get current time
    String timeString = _timeManager->getCurrentTime();
    
    // Update connection time
    _connectionTime = timeString;
    
    // Create online status message
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = _deviceId;
    doc["IsConnected"] = true;
    doc["LastSeen"] = timeString;
    doc["ConnectedSince"] = _connectionTime;
    
    String message;
    serializeJson(doc, message);
    
    // Publish online status
    return _connectionManager->publish(_statusTopic, message.c_str(), true);
}

bool DeviceStatusManager::updateDeviceStatus() {
    if (!_connectionManager->isConnected()) {
        return false;
    }
    
    // Get current time for LastSeen
    String timeString = _timeManager->getCurrentTime();
    
    // Create status document
    DynamicJsonDocument doc(256);
    doc["DeviceName"] = _deviceId;
    doc["IsConnected"] = true;
    doc["LastSeen"] = timeString;
    doc["ConnectedSince"] = _connectionTime;
    
    String message;
    serializeJson(doc, message);
    
    // Update status
    return _connectionManager->publish(_statusTopic, message.c_str(), true);
}

void DeviceStatusManager::checkStatusUpdate() {
    if (!_connectionManager->isConnected()) {
        return;
    }
    
    unsigned long now = millis();
    if (now - _lastStatusUpdate > _statusUpdateInterval) {
        _lastStatusUpdate = now;
        updateDeviceStatus();
    }
}

void DeviceStatusManager::resetConnectionTime() {
    _connectionTime = "";
}

void DeviceStatusManager::updateConnectionTime() {
    _connectionTime = _timeManager->getCurrentTime();
}

bool DeviceStatusManager::clearRetainedStatus() {
    return _connectionManager->publish(_statusTopic, "", true);
}