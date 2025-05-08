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
        private readonly TimeSpan _deviceTimeout = TimeSpan.FromSeconds(30);

        public DeviceConnectionTracker(ILogger<DeviceConnectionTracker> logger)
        {
            _logger = logger;
            _connectionCheckTimer = new Timer(CheckConnections, null,
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        private class DeviceStatus
        {
            public DateTime LastSeen { get; set; }
            public bool IsConnected { get; set; }
        }

        public void UpdateDeviceStatus(string deviceId, DateTime timestamp)
        {
            _deviceStatuses.AddOrUpdate(
                deviceId,
                _ => CreateNewDeviceStatus(deviceId, timestamp),
                (_, existing) => UpdateExistingDeviceStatus(deviceId, existing, timestamp)
            );
        }

        private DeviceStatus CreateNewDeviceStatus(string deviceId, DateTime timestamp)
        {
            _logger.LogInformation("Device {DeviceId} connected", deviceId);
            return new DeviceStatus { LastSeen = timestamp, IsConnected = true };
        }

        private DeviceStatus UpdateExistingDeviceStatus(string deviceId, DeviceStatus existing, DateTime timestamp)
        {
            if (!existing.IsConnected)
                _logger.LogInformation("Device {DeviceId} reconnected", deviceId);
            
            existing.LastSeen = timestamp;
            existing.IsConnected = true;
            return existing;
        }

        private void CheckConnections(object? state)
        {
            try
            {
                var currentTime = DateTime.UtcNow;
                
                foreach (var entry in _deviceStatuses)
                {
                    string deviceId = entry.Key;
                    DeviceStatus status = entry.Value;
                    
                    if (HasConnectionTimedOut(status, currentTime))
                    {
                        MarkDeviceAsDisconnected(deviceId, status, currentTime);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking connections");
            }
        }
        
        private bool HasConnectionTimedOut(DeviceStatus status, DateTime currentTime)
        {
            return status.IsConnected && (currentTime - status.LastSeen > _deviceTimeout);
        }
        
        private void MarkDeviceAsDisconnected(string deviceId, DeviceStatus status, DateTime currentTime)
        {
            status.IsConnected = false;
            _logger.LogWarning("Device {DeviceId} connection lost at {Timestamp}", deviceId, currentTime);
        }

        public void Dispose() => _connectionCheckTimer?.Dispose();
    }
}