using Core.Domain.TestEntities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDeviceRepository
{
    
    public void SaveDevices(TestDevices devices);
    public bool DeviceExists(Guid deviceId);
    
    public void RegisterNewDevice(Guid deviceId, string deviceName, DateTime lastSeen);
}