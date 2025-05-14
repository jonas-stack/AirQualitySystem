using Application.Interfaces.Infrastructure.MQTT;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
using Core.Domain.Constants;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DataValidator : IDataValidator
{
    private readonly ILogger<DataValidator> _logger;

    public DataValidator(ILogger<DataValidator> logger)
    {
        _logger = logger;
    }
    
    public bool IsDataComplete<T>(T dto) where T : class
    {
        try
        {
            if (dto is SensorDataDto sensorDataDto)
            {
                return HasValidSensorData(sensorDataDto) && HasValidSensorReadingTimestamp(sensorDataDto);
            }
            else if (dto is DeviceDto deviceDto)
            {
                return HasValidDeviceStatus(deviceDto) && HasValidDeviceLastSeenTimestamp(deviceDto);
            }
            else
            {
                _logger.LogWarning("Unsupported DTO type: {Type}", typeof(T).Name);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating data completeness");
            return false;
        }
    }

    public bool IsIdValid<T>(T dto) where T : class
    {
        try
        {
            if (dto is SensorDataDto sensorDataDto)
            {
                return DataTypeConverter.GetDeviceGuid(sensorDataDto.DeviceId) != Guid.Empty;
            }
            else if (dto is DeviceDto deviceDto)
            {
                return DataTypeConverter.GetDeviceGuid(deviceDto.DeviceName) != Guid.Empty;
            }
            else
            {
                _logger.LogWarning("Unsupported DTO type: {Type}", typeof(T).Name);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating ID");
            return false;
        }
    }

    public bool IsTimeStampValid<T>(T dto) where T : class
    {
        try
        {
            if (dto is SensorDataDto sensorDataDto)
            {
                return HasValidSensorReadingTimestamp(sensorDataDto);
            }
            else if (dto is DeviceDto deviceDto)
            {
                return HasValidDeviceLastSeenTimestamp(deviceDto);
            }
            else
            {
                _logger.LogWarning("Unsupported DTO type: {Type}", typeof(T).Name);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating timestamp");
            return false;
        }
    }
    
    private bool HasValidSensorData(SensorDataDto dto)
    {
        return !string.IsNullOrWhiteSpace(dto.DeviceId) &&
               dto.Temperature >= ValidationConstants.MIN_TEMPERATURE && 
               dto.Temperature <= ValidationConstants.MAX_TEMPERATURE &&
               dto.Humidity >= ValidationConstants.MIN_HUMIDITY && 
               dto.Humidity <= ValidationConstants.MAX_HUMIDITY &&
               dto.AirQuality >= ValidationConstants.MIN_AIR_QUALITY &&
               dto.Pm25 >= ValidationConstants.MIN_PM25;
    }

    private bool HasValidDeviceStatus(DeviceDto dto)
    {
        return !string.IsNullOrWhiteSpace(dto.DeviceName) &&
               dto.LastSeen != default;
    }

    private bool HasValidDeviceLastSeenTimestamp(DeviceDto dto)
    {
        var currentTime = DateTimeOffset.UtcNow;
        var maxPastTime = currentTime.Subtract(ValidationConstants.MAX_DATA_AGE);
        var maxFutureTime = currentTime.Add(ValidationConstants.MAX_FUTURE_OFFSET);
    
        var deviceTime = DateTimeOffset.FromUnixTimeSeconds(dto.LastSeen);
    
        return dto.LastSeen != default && 
               deviceTime >= maxPastTime &&
               deviceTime <= maxFutureTime;
    }

    private bool HasValidSensorReadingTimestamp(SensorDataDto dto)
    {
        var currentTime = DateTimeOffset.UtcNow;
        var maxPastTime = currentTime.Subtract(ValidationConstants.MAX_DATA_AGE);
        var maxFutureTime = currentTime.Add(ValidationConstants.MAX_FUTURE_OFFSET);
    
        var sensorTime = DateTimeOffset.FromUnixTimeSeconds(dto.TimestampUnix);
    
        return dto.TimestampUnix != default &&
               sensorTime >= maxPastTime &&
               sensorTime <= maxFutureTime;
    }
}