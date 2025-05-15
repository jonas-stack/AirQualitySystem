using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.TestEntities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class DeviceRepository : IDeviceRepository
{
    
    private readonly MyDbContextDocker _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;
    
    public DeviceRepository(MyDbContextDocker dbContext, ILogger<SensorDataRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public void SaveDevices(TestDevices devices)
    {
        try
        {
            // Check if the device already exists
            var existingDevice = _dbContext.TestDevices.Find(devices.DeviceId);
        
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
            
            var newDeviceHistory = new TestDeviceConnectionHistory()
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
}