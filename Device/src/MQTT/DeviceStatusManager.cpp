#include "DeviceStatusManager.h"
#include "JsonSerializer.h"

DeviceStatusManager::DeviceStatusManager(ConnectionManager& connectionManager, 
                                       TimeManager& timeManager,
                                       JsonSerializer& jsonSerializer,
                                       const char* deviceId,
                                       const char* statusTopic,
                                       unsigned long updateInterval) :
    _connectionManager(connectionManager),
    _jsonSerializer(jsonSerializer),
    _timeManager(timeManager),
    _deviceId(deviceId),
    _lastStatusUpdate(0),
    _statusUpdateInterval(updateInterval),
    _statusTopic(statusTopic) {
}

String DeviceStatusManager::prepareOfflineMessage() {
    String timeString = _timeManager.getCurrentTime();
    if (_connectionTime.isEmpty()) {
        _connectionTime = timeString;
    }
    // Use JsonSerializer for status message
    return _jsonSerializer.serializeStatusMessage("offline", _deviceId);
}

bool DeviceStatusManager::publishOnlineStatus() {
    String timeString = _timeManager.getCurrentTime();
    _connectionTime = timeString;
    // Use JsonSerializer for status message
    String message = _jsonSerializer.serializeStatusMessage("online", _deviceId);
    return _connectionManager.publish(_statusTopic, message.c_str(), true);
}

bool DeviceStatusManager::updateDeviceStatus() {
    if (!_connectionManager.isConnected()) {
        return false;
    }
    // Use JsonSerializer for status message
    String message = _jsonSerializer.serializeStatusMessage("online", _deviceId);
    return _connectionManager.publish(_statusTopic, message.c_str(), true);
}

void DeviceStatusManager::checkStatusUpdate() {
    if (!_connectionManager.isConnected()) {
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
    _connectionTime = _timeManager.getCurrentTime();
}

bool DeviceStatusManager::clearRetainedStatus() {
    return _connectionManager.publish(_statusTopic, "", true);
}