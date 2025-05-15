using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT
{
    public class DeviceConnectionTracker
    {
        private readonly ConcurrentDictionary<string, (DateTime LastSeen, bool IsConnected)> _devices = new();
        private readonly ILogger<DeviceConnectionTracker> _logger;

        public DeviceConnectionTracker(ILogger<DeviceConnectionTracker> logger)
        {
            _logger = logger;
        }

        public void UpdateDeviceStatus(Guid deviceId, DateTime timestamp, bool isConnected = true)
        {
            var deviceKey = deviceId.ToString();
            var previousState = _devices.TryGetValue(deviceKey, out var status) ? status.IsConnected : false;

            // Update with the connection state from the message
            _devices[deviceKey] = (timestamp, isConnected);

            // Log connection changes
            if (isConnected && !previousState)
            {
                _logger.LogInformation("Device {DeviceId} connected", deviceId);
            }
            else if (!isConnected && previousState)
            {
                _logger.LogWarning("Device {DeviceId} disconnected at {Timestamp}", 
                    deviceId, DateTime.Now);
            }
        }

        public bool IsDeviceConnected(Guid deviceId)
        {
            return _devices.TryGetValue(deviceId.ToString(), out var status) && status.IsConnected;
        }
    }
}