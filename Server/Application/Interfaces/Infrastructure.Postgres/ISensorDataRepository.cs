using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface ISensorDataRepository
{
    
    Task SaveSensorDataAsync(SensorData sensorData);

    Task<PagedResult<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId, int pageNumber = 1, int pageSize = 50);
}