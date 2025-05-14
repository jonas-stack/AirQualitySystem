using System.Text.Json;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
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
    
    public DeviceConnectionHandler(
        ILogger<DeviceConnectionHandler> logger,
        DeviceConnectionTracker connectionTracker,
        IDeviceRepository deviceRepository, IDataValidator validator, DevicesMapper mapper)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _deviceRepository = deviceRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public string TopicFilter => "airquality/status";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            var payload = args.PublishMessage.Payload;
            if (payload == null) return;

            var json = System.Text.Encoding.UTF8.GetString(payload);
                
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogInformation("Received empty status message");
                return;
            }
        
            var dto = JsonSerializer.Deserialize<DeviceDto>(json);
            if (dto == null) return;
            
            var timestamp = DataTypeConverter.GetLocalDateTime(dto.LastSeen);
            var deviceGuid = DataTypeConverter.GetDeviceGuid(dto.DeviceName);
            _connectionTracker.UpdateDeviceStatus(deviceGuid, timestamp);

            // Validate data before saving using the dedicated validator
            if (!_validator.IsDataComplete(dto))
            {
                _logger.LogWarning("Incomplete data received from device {DeviceId}. Skipping save.", dto.DeviceGuid);
                return;
            }
                
            // Use mapper to create entity
            var entity = _mapper.MapToTestEntity(dto); //TODO CHANGE TO ACTUAL ENTITY FORM MAPPER
            _deviceRepository.SaveDevices(entity);
                
            _logger.LogInformation("Data saved successfully for device: {DeviceId}", entity.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}