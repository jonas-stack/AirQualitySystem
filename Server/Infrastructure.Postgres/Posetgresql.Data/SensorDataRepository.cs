using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;

    public SensorDataRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public void SaveSensorData(SensorData sensorData)
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