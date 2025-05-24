using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;

namespace Application.Interfaces;

public interface ISensorDataService
{
    Task<PagedResult<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId, int pageNumber = 1, int pageSize = 50);
}