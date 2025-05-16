using System.Text.Json;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Utility;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers;

public class DeviceConnectionHandler : IMqttMessageHandler
{

    private readonly ILogger<DeviceConnectionHandler> _logger;
    private readonly DeviceConnectionTracker _connectionTracker;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDataValidator _validator;
    private readonly DevicesMapper _mapper;
    private readonly IMqttMessageDeserializer _deserializer;
    
    public DeviceConnectionHandler(
        ILogger<DeviceConnectionHandler> logger,
        DeviceConnectionTracker connectionTracker,
        IDeviceRepository deviceRepository, IDataValidator validator, DevicesMapper mapper, IMqttMessageDeserializer deserializer)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _deviceRepository = deviceRepository;
        _validator = validator;
        _mapper = mapper;
        _deserializer = deserializer;
    }

    public string TopicFilter => "airquality/status";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            var payload = args.PublishMessage.Payload;
            if (payload == null) return;

            var result = _deserializer.Deserialize<DeviceDto>(payload);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed to deserialize message: {Error}. Raw content: {Content}", 
                    result.ErrorMessage, result.RawContent);
                return;
            }

            var dto = result.Data!;
            
            // Validate data before processing
            if (!_validator.IsIdValid(dto))
            {
                _logger.LogWarning("Invalid device ID format for device {DeviceId}. Skipping save.", dto.DeviceName);
                return;
            }

            if (!_validator.IsTimeStampValid(dto))
            {
                _logger.LogWarning("Invalid timestamp for device {DeviceId}. Skipping save.", dto.DeviceName);
                return;
            }

            if (!_validator.IsDataComplete(dto))
            {
                _logger.LogWarning("Incomplete device data for device {DeviceId}. Skipping save.", dto.DeviceName);
                return;
            }
            
            // Only update connection status if data is valid
            var timestamp = DataTypeConverter.GetLocalDateTime(dto.LastSeen);
            var deviceGuid = DataTypeConverter.GetDeviceGuid(dto.DeviceName);
            _connectionTracker.UpdateDeviceStatus(deviceGuid, timestamp, dto.IsConnected);
                
            // Use mapper to create entity
            var entity = _mapper.MapToEntity(dto); //TODO CHANGE TO ACTUAL ENTITY FORM MAPPER
            _deviceRepository.SaveDevices(entity);
                
            _logger.LogInformation("Device status saved successfully for device: {DeviceId}", entity.DeviceId);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}