using System.Text;
using System.Text.Json;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Services;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers;

public class SensorDataHandler : IMqttMessageHandler
{
    private readonly DeviceConnectionTracker _connectionTracker;
    private readonly ILogger<SensorDataHandler> _logger;
    private readonly SensorDataMapper _mapper;
    private readonly ISensorDataRepository _sensorDataRepository;
    private readonly DataValidator _validator;

    public SensorDataHandler(
        ILogger<SensorDataHandler> logger,
        DeviceConnectionTracker connectionTracker,
        DataValidator validator,
        SensorDataMapper mapper,
        ISensorDataRepository sensorDataRepository)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _validator = validator;
        _mapper = mapper;
        _sensorDataRepository = sensorDataRepository;
    }

    public string TopicFilter => "AirQuality/Data";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            var payload = args.PublishMessage.Payload;
            if (payload == null) return;

            var json = Encoding.UTF8.GetString(payload);

            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogInformation("Received empty Data message. Likely a retained message clear operation.");
                return;
            }

            var dto = JsonSerializer.Deserialize<SensorDataDto>(json);
            if (dto == null) return;


            var timestamp = DataTypeConverter.GetLocalDateTime(dto.TimestampUnix);
            var deviceGuid = DataTypeConverter.GetDeviceGuid(dto.DeviceId);
            _connectionTracker.UpdateDeviceStatus(deviceGuid, timestamp);

            // Validate data before saving using the dedicated validator
            if (!_validator.IsDataComplete(dto))
            {
                _logger.LogWarning("Incomplete data received from device {DeviceId}. Skipping save.", dto.DeviceId);
                return;
            }

            // Use mapper to create entity
            var entity = _mapper.MapToTestEntity(dto); //TODO CHANGE TO ACTUAL ENTITY FORM MAPPER
            _sensorDataRepository.SaveSensorData(entity);

            _logger.LogInformation("Data saved successfully for device: {DeviceId}", entity.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}