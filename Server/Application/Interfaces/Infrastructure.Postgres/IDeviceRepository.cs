using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDeviceRepository
{
    
    public void SaveDevices(Devices devices);
    public bool DeviceExists(Guid deviceId);
    
    public void RegisterNewDevice(Guid deviceId, string deviceName, DateTime lastSeen);
}