using Core.Domain.TestEntities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface ISensorDataRepository
{
    
    public void SaveSensorData (TestSensorData sensorData);
    
}