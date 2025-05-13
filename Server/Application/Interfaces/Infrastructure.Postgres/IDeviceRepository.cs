using Core.Domain.TestEntities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDeviceRepository
{
    public void SaveConnectionEvent(Guid deviceGuid, String deviceName, DateTime timeStamp, bool isConnected);
    
    public void SaveSensorData (TestSensorData sensorData);

    public void SaveLoginValidPayload(string? deviceId, string rawPayload, string errorReason);
}