using Application.Interfaces.Infrastructure.MQTT;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
using Core.Domain.Constants;
using HiveMQtt.Client.Events;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SensorDataValidator : IDataValidator<SensorDataDto>
{
    private readonly ILogger<SensorDataValidator> _logger;
    private readonly SensorDataMapper _mapper;

    public SensorDataValidator(ILogger<SensorDataValidator> logger, SensorDataMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    // Original method for backward compatibility
    public bool IsDataComplete(SensorDataDto dto)
    {
        return HasValidSensorReadings(dto) &&
               HasValidDeviceId(dto) &&
               HasValidTimestamp(dto, null);
    }

    // Overload that accepts message args
    public bool IsDataComplete(SensorDataDto dto, OnMessageReceivedEventArgs args)
    {
        return HasValidSensorReadings(dto) &&
               HasValidDeviceId(dto) &&
               HasValidTimestamp(dto, args);
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

    
    private bool HasValidTimestamp(SensorDataDto dto, OnMessageReceivedEventArgs? args)
    {
        var messageTime = _mapper.GetLocalDateTime(dto.TimestampUnix);
        
        // Use server received time if available, otherwise use current time
        DateTime referenceTime = DateTime.Now;
        
        if (args?.PublishMessage.UserProperties != null && 
            args.PublishMessage.UserProperties.TryGetValue("ServerReceivedTime", out string? receivedTimeStr))
        {
            referenceTime = DateTime.Parse(receivedTimeStr);
        }

        if (messageTime < referenceTime.Subtract(ValidationConstants.MAX_DATA_AGE))
        {
            _logger.LogWarning("Data too old: {Timestamp}", messageTime);
            return false;
        }

        if (messageTime > referenceTime.Add(ValidationConstants.MAX_FUTURE_OFFSET))
        {
            _logger.LogWarning(
                "Future timestamp detected: Message time from ESP32: {MessageTime}, Reference time: {ReferenceTime}, Difference: {DifferenceSeconds}s",
                messageTime,
                referenceTime,
                (messageTime - referenceTime).TotalSeconds
            );
            return false;
        }

        return true;
    }
}