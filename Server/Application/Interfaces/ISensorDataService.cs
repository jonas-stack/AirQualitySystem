using Application.Models.Dtos.MQTT;

namespace Application.Interfaces;

public interface ISensorDataService
{
    Task<List<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId);
}