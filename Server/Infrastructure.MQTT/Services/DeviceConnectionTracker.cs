using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.Services
{
    public class DeviceConnectionTracker : IDisposable
    {
        private readonly ConcurrentDictionary<string, DeviceStatus> _deviceStatuses = new();
        private readonly ILogger<DeviceConnectionTracker> _logger;
        private readonly Timer _connectionCheckTimer;
        private readonly TimeSpan _deviceTimeout = TimeSpan.FromSeconds(30); // 30 seconds until timeout

        public DeviceConnectionTracker(ILogger<DeviceConnectionTracker> logger)
        {
            _logger = logger;
            _logger.LogInformation("DeviceConnectionTracker initialized with timeout: {TimeoutSeconds}s", _deviceTimeout.TotalSeconds);
            
            // Check every 5 seconds for faster detection
            _connectionCheckTimer = new Timer(CheckConnections, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        public void UpdateDeviceStatus(string deviceId, DateTime timestamp)
        {
            _deviceStatuses.AddOrUpdate(
                deviceId,
                _ => {
                    _logger.LogInformation("Device {DeviceId} connected for the first time", deviceId);
                    return new DeviceStatus { LastSeen = timestamp, IsConnected = true };
                },
                (_, existing) => {
                    if (!existing.IsConnected)
                    {
                        _logger.LogInformation("Device {DeviceId} reconnected after being offline", deviceId);
                    }
                    existing.LastSeen = timestamp;
                    existing.IsConnected = true;
                    return existing;
                }
            );
        }

        public bool IsDeviceConnected(string deviceId)
        {
            return _deviceStatuses.TryGetValue(deviceId, out var status) && status.IsConnected;
        }

        private void CheckConnections(object? state)
        {
            try
            {
                var now = DateTime.UtcNow;
                foreach (var device in _deviceStatuses)
                {
                    if (device.Value.IsConnected && now - device.Value.LastSeen > _deviceTimeout)
                    {
                        device.Value.IsConnected = false;
                        _logger.LogWarning("Device {DeviceId} connection lost. Last seen: {TimeSinceLastSeen} seconds ago", 
                            device.Key, (now - device.Value.LastSeen).TotalSeconds);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking device connections");
            }
        }

        public void Dispose()
        {
            _connectionCheckTimer?.Dispose();
        }

        private class DeviceStatus
        {
            public DateTime LastSeen { get; set; }
            public bool IsConnected { get; set; }
        }
    }
}