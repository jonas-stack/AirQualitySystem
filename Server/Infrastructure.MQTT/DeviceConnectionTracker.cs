using System.Collections.Concurrent;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT
{
    public class DeviceConnectionTracker : IDisposable
    {
        private readonly ConcurrentDictionary<string, (DateTime LastSeen, bool IsConnected)> _devices = new();
        private readonly ILogger<DeviceConnectionTracker> _logger;
        private readonly System.Timers.Timer _connectionCheckTimer;
        private readonly TimeSpan _deviceTimeout = TimeSpan.FromMinutes(6); //6 minutes before device connection is lost log

        public DeviceConnectionTracker(ILogger<DeviceConnectionTracker> logger)
        {
            _logger = logger;
            
            _connectionCheckTimer = new System.Timers.Timer(30000); // 30 seconds between checks for activity of device
            _connectionCheckTimer.Elapsed += (_, _) => CheckConnections();
            _connectionCheckTimer.Start();
        }

        public void UpdateDeviceStatus(string deviceId, DateTime timestamp)
        {
            var wasConnected = _devices.TryGetValue(deviceId, out var status) && status.IsConnected;
            
            _devices[deviceId] = (timestamp, true);
            
            if (!wasConnected)
                _logger.LogInformation("Device {DeviceId} {ConnectionStatus}", 
                    deviceId, wasConnected ? "reconnected" : "connected");
        }

        private void CheckConnections()
        {
            try
            {
                var currentTime = DateTime.Now;
                
                foreach (var (deviceId, (lastSeen, isConnected)) in _devices)
                {
                    if (isConnected && (currentTime - lastSeen > _deviceTimeout))
                    {
                        _devices[deviceId] = (lastSeen, false);
                        _logger.LogWarning("Device {DeviceId} connection lost at {Timestamp}", 
                            deviceId, currentTime);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking connections");
            }
        }

        public void Dispose() => _connectionCheckTimer?.Dispose();
    }
}