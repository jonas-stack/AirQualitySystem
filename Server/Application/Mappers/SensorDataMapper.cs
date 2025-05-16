using Application.Interfaces.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Utility;
using Core.Domain.Entities;

namespace Application.Mappers;

public class SensorDataMapper : ISensorDataMapper
{
    public SensorData MapToEntity(SensorDataDto dto)
    {
        return new SensorData
        {
            Temperature = dto.Temperature,
            Humidity = dto.Humidity,
            AirQuality = dto.AirQuality,
            Pm25 = dto.Pm25,
            DeviceId = DataTypeConverter.GetDeviceGuid(dto.DeviceId),
            Timestamp = DataTypeConverter.GetLocalDateTime(dto.TimestampUnix)
        };
    }
}