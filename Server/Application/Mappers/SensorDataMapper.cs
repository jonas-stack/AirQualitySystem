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

    public SensorDataDto MapToDto(SensorData entity)
    {
        // vælger at runde ned fordi vi primært bruger det her i frontend
        // det her er "readonly" data og intet andet
        return new SensorDataDto
        {
            Temperature = Math.Round(entity.Temperature, 2),
            Humidity = Math.Round(entity.Humidity, 2),
            AirQuality = Math.Round(entity.AirQuality, 2),
            Pm25 = Math.Round(entity.Pm25, 2),
            DeviceId = entity.DeviceId.ToString(),
            TimestampUnix = entity.Timestamp.Ticks
        };
    }
}