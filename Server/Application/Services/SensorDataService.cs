using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;

namespace Application.Services;

public class SensorDataService(IDeviceRepository deviceRepository, ISensorDataRepository sensorDataRepository) : ISensorDataService
{
    public async Task<PagedResult<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId, int pageNumber = 1, int pageSize = 50)
    {
        // den her thrower hvis fejl, så ingen grund til null checks
        await deviceRepository.GetDevice(deviceId);
        return await sensorDataRepository.GetSensorDataForDeviceAsync(deviceId, pageNumber, pageSize);
    }
}