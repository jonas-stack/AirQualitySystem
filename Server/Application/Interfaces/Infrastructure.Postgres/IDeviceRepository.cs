using Core.Domain.TestEntities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDeviceRepository
{
    
    public void SaveSensorData (TestSensorData sensorData);
    
}