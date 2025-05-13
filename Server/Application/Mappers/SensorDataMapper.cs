using Application.Models.Dtos.MQTT;
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
            DeviceId = GetDeviceGuid(dto.DeviceId),
            Timestamp = GetLocalDateTime(dto.TimestampUnix)
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
            DeviceId = GetDeviceGuid(dto.DeviceId),
            Timestamp = GetLocalDateTime(dto.TimestampUnix)
        };
    }
    
    private Guid GetDeviceGuid(string deviceId)
    {
        // Move the GUID conversion logic here
        if (Guid.TryParse(deviceId, out Guid deviceGuid))
            return deviceGuid;
            
        return CreateDeterministicGuid(deviceId);
    }

    public DateTime GetLocalDateTime(long timestampUnix)
    {
        // Convert Unix timestamp to UTC DateTime
        var utcTime = DateTimeOffset.FromUnixTimeSeconds(timestampUnix).DateTime;
        
        //added 1 minute to timestamp as sync between device internal clock and pc can be of
        if (utcTime > DateTime.UtcNow && utcTime <= DateTime.UtcNow.AddMinutes(1)) //TODO REMOVE THIS AS THE VALIDATION CONSTANT MAX_OFFSET SHOULD HANDLE THIS
        {
            utcTime = DateTime.UtcNow;
        }
    
        // Convert from UTC to local time zone
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime,
            TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
    }
    
    private Guid CreateDeterministicGuid(string deviceId)
    {
        // Moved from DTO
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(deviceId);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }
}