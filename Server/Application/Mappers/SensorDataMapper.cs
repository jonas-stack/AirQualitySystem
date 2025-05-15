using Application.Models.Dtos.MQTT;
using Application.Utility;
using Core.Domain.Entities;
using Core.Domain.TestEntities;

namespace Application.Mappers;

public class SensorDataMapper
{
    public TestSensorData MapToTestEntity(SensorDataDto dto)
    {
        return new TestSensorData
        {
            Temperature = dto.Temperature,
            Humidity = dto.Humidity,
            AirQuality = dto.AirQuality,
            Pm25 = dto.Pm25,
            DeviceId = DataTypeConverter.GetDeviceGuid(dto.DeviceId),
            Timestamp = DataTypeConverter.GetLocalDateTime(dto.TimestampUnix)
        };
    }

    public SensorData MapToProductionEntity(SensorDataDto dto)
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