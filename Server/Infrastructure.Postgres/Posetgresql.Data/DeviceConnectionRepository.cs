using Application.Interfaces.Infrastructure.Postgres;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class DeviceConnectionRepository : IDeviceConnectionRepository
{
    public void SaveConnectionEvent(Guid deviceGuid, string deviceName, DateTime timeStamp, bool isConnected)
    {
        throw new NotImplementedException();
    }
}