namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDeviceConnectionRepository
{
    
    public void SaveConnectionEvent(Guid deviceGuid, String deviceName, DateTime timeStamp, bool isConnected);
    
}