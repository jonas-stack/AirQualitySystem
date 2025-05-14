using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.TestEntities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class DeviceRepository : IDeviceRepository
{
    private readonly MyDbContextDocker _dbContext;
    private readonly ILogger<DeviceRepository> _logger;

    public DeviceRepository(MyDbContextDocker dbContext, ILogger<DeviceRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public void SaveSensorData(TestSensorData sensorData)
    {
        try
        {
            _dbContext.Add(sensorData);
            _dbContext.SaveChanges();
            _logger.LogDebug("Sensor data saved successfully for device: {DeviceId}", sensorData.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving sensor data for device {DeviceId}", sensorData.DeviceId);
            throw; // Re-throw to allow higher layers to handle or report the error
        }
    }
}