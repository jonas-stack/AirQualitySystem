using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Utility;
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
    private readonly IDataValidator _validator;
    private readonly IMqttMessageDeserializer _deserializer;
    private readonly IDeviceRepository _deviceRepository;

    public SensorDataHandler(
        ILogger<SensorDataHandler> logger,
        DeviceConnectionTracker connectionTracker,
        IDataValidator validator,
        SensorDataMapper mapper,
        ISensorDataRepository sensorDataRepository, IMqttMessageDeserializer deserializer, IDeviceRepository deviceRepository)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _validator = validator;
        _mapper = mapper;
        _sensorDataRepository = sensorDataRepository;
        _deserializer = deserializer;
        _deviceRepository = deviceRepository;
    }

    public string TopicFilter => "AirQuality/Data";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            var payload = args.PublishMessage.Payload;
            if (payload == null) return;

            var result = _deserializer.Deserialize<SensorDataDto>(payload);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed to deserialize message: {Error}. Raw content: {Content}", 
                    result.ErrorMessage, result.RawContent);
                return;
            }

            var dto = result.Data!;
            
            // Validate data before saving
            if (!_validator.IsIdValid(dto))
            {
                _logger.LogWarning("Invalid device ID format for device {DeviceId}. Skipping save.", dto.DeviceId);
                return;
            }

            if (!_validator.IsTimeStampValid(dto)) 
            {
                _logger.LogWarning("Invalid timestamp for device {DeviceId}. Skipping save.", dto.DeviceId);
                return;
            }

            if (!_validator.IsDataComplete(dto))
            {
                _logger.LogWarning("Incomplete sensor data for device {DeviceId}. Skipping save.", dto.DeviceId);
                return;
            }
            
            var timestamp = DataTypeConverter.GetLocalDateTime(dto.TimestampUnix);
            var deviceGuid = DataTypeConverter.GetDeviceGuid(dto.DeviceId);
            
            if (!_deviceRepository.DeviceExists(deviceGuid))
            {
                _logger.LogInformation("New device detected with ID {DeviceId}. Auto-registering.", dto.DeviceId);
                _deviceRepository.RegisterNewDevice(deviceGuid, dto.DeviceId, timestamp);
            }
            
            _connectionTracker.UpdateDeviceStatus(deviceGuid, timestamp);
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