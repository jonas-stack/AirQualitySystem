using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface ISensorDataRepository
{
    
    Task SaveSensorDataAsync(SensorData sensorData);
    
}