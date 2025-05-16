using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class DeviceRepository : IDeviceRepository
{
    
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;
    
    public DeviceRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public void SaveDevices(Devices devices)
    {
        try
        {
            // Check if the device already exists
            var existingDevice = _dbContext.Devices.Find(devices.DeviceId);
            
            // For disconnection events, use current time rather than the reported LastSeen
            if (!devices.IsConnected)
            {
                // Create DateTime with Kind.Unspecified to avoid PostgreSQL error
                devices.LastSeen = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            }
            else 
            {
                // Ensure connected events also have the correct Kind
                devices.LastSeen = new DateTime(devices.LastSeen.Ticks, DateTimeKind.Unspecified);
            }
        
            if (existingDevice != null)
            {
                // Update existing device
                existingDevice.DeviceName = devices.DeviceName;
                existingDevice.IsConnected = devices.IsConnected;
                existingDevice.LastSeen = devices.LastSeen;
                _dbContext.Update(existingDevice);
            }
            else
            {
                // Add new device
                _dbContext.Add(devices);
            }
            
            var newDeviceHistory = new DeviceConnectionHistory()
            {
                DeviceId = devices.DeviceId,
                IsConnected = devices.IsConnected,
                LastSeen = devices.LastSeen
            };

            _dbContext.Add(newDeviceHistory);
        
            _dbContext.SaveChanges();
            _logger.LogDebug("Device saved successfully: {DeviceId}", devices.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving device {DeviceId}", devices.DeviceId);
            throw;
        }
    }

    public bool DeviceExists(Guid deviceId)
    {
        try
        {
            return _dbContext.Devices.Any(d => d.DeviceId == deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if device {DeviceId} exists", deviceId);
            throw;
        }
    }

    public void RegisterNewDevice(Guid deviceId, string deviceName, DateTime lastSeen)
    {
        try
        {
            var newDevice = new Devices
            {
                DeviceId = deviceId,
                DeviceName = deviceName,
                IsConnected = true,
                LastSeen = new DateTime(lastSeen.Ticks, DateTimeKind.Unspecified)
            };
            
            SaveDevices(newDevice);
            _logger.LogInformation("Device {DeviceId} automatically registered", deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering new device {DeviceId}", deviceId);
            throw;
        }
    }
}