using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.MQTT;

namespace Application.Services;

public class SensorDataService(IDeviceRepository deviceRepository, ISensorDataRepository sensorDataRepository) : ISensorDataService
{

    public async Task<List<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId)
    {
        var exists = deviceRepository.get
    }
}