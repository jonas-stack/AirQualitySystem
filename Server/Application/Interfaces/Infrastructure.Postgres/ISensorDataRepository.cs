using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface ISensorDataRepository
{
    
    public void SaveSensorData (SensorData sensorData);
    
}