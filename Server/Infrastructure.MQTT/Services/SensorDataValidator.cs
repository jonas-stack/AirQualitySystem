using Application.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.Services;

public class SensorDataValidator
{
    private readonly ILogger _logger;

    public SensorDataValidator(ILogger logger)
    {
        _logger = logger;
    }

    public bool IsDataComplete(SensorDataDto dto)
    {
        return HasValidSensorReadings(dto) &&
               HasValidDeviceId(dto) &&
               HasValidTimestamp(dto);
    }

    private bool HasValidSensorReadings(SensorDataDto dto)
    {
        if (dto.Temperature <= ValidationConstants.MIN_TEMPERATURE ||
            dto.Temperature >= ValidationConstants.MAX_TEMPERATURE)
        {
            _logger.LogWarning("Invalid temperature: {Temperature}", dto.Temperature);
            return false;
        }

        if (dto.Humidity < ValidationConstants.MIN_HUMIDITY ||
            dto.Humidity > ValidationConstants.MAX_HUMIDITY)
        {
            _logger.LogWarning("Invalid humidity: {Humidity}", dto.Humidity);
            return false;
        }

        if (dto.AirQuality <= ValidationConstants.MIN_AIR_QUALITY)
        {
            _logger.LogWarning("Invalid air quality: {AirQuality}", dto.AirQuality);
            return false;
        }

        if (dto.Pm25 < ValidationConstants.MIN_PM25)
        {
            _logger.LogWarning("Invalid PM2.5: {Pm25}", dto.Pm25);
            return false;
        }

        return true;
    }

    private bool HasValidDeviceId(SensorDataDto dto)
    {
        if (string.IsNullOrEmpty(dto.DeviceId))
        {
            _logger.LogWarning("Missing device ID");
            return false;
        }

        return true;
    }

    private bool HasValidTimestamp(SensorDataDto dto)
    {
        var messageTime = dto.GetDateTime();
        var now = DateTime.UtcNow;

        if (messageTime < now.Subtract(ValidationConstants.MAX_DATA_AGE))
        {
            _logger.LogWarning("Data too old: {Timestamp}", messageTime);
            return false;
        }

        if (messageTime > now.Add(ValidationConstants.MAX_FUTURE_OFFSET))
        {
            _logger.LogWarning("Future timestamp detected: {Timestamp}", messageTime);
            return false;
        }

        return true;
    }
}