using Application.Interfaces;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Utility;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Infrastructure.MQTT.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.Handlers;

public class DeviceConnectionHandler : IMqttMessageHandler
{
    private readonly DeviceConnectionTracker _connectionTracker;
    private readonly IJsonDeserializer _deserializer;
    private readonly IDeviceRepository _deviceRepository;

    private readonly ILogger<DeviceConnectionHandler> _logger;
    private readonly IDevicesMapper _mapper;
    private readonly IDataValidator _validator;
    private readonly IDeviceService _deviceService;

    public DeviceConnectionHandler(
        ILogger<DeviceConnectionHandler> logger,
        DeviceConnectionTracker connectionTracker,
        IDeviceRepository deviceRepository, IDataValidator validator, IDevicesMapper mapper,
        IJsonDeserializer deserializer,
        IDeviceService deviceService)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _deviceRepository = deviceRepository;
        _validator = validator;
        _mapper = mapper;
        _deserializer = deserializer;
        _deviceService = deviceService;
    }

    public string TopicFilter => "airquality/status";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public async Task HandleAsync(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            var payload = args.PublishMessage.Payload;
            if (payload == null) return;
            
            // Deserialize json payload fra mqtt til DeviceDto
            var result = _deserializer.Deserialize<DeviceDto>(payload);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to deserialize message: {Error}. Raw content: {Content}",
                    result.ErrorMessage, result.RawContent);
                return;
            }

            var dto = result.Data!;

            // valider data i dto før det kan gemmes
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
            
            // brug mapper til at omdanne dto til entity
            var entity = _mapper.MapToEntity(dto); //TODO CHANGE TO ACTUAL ENTITY FORM MAPPER
            
            // send en entity til repository for at gemme i database
            await _deviceRepository.SaveDevicesAsync(entity);
            
            // opdaterer device status i connection tracker
             _connectionTracker.UpdateDeviceStatus(entity.DeviceId, entity.LastSeen, dto.IsConnected);
            
            // broadcast for at sørge for at klienten får opdatering om nuværende status
            // ID'et kommer fra entity når den er gemt i databasen, før det ved vi ikke hvad ID er
            // for at sørge for at entity igen får korrekt ID, sætter vi fra entity til dto.
            dto.DeviceGuid = entity.DeviceId.ToString();
            await _deviceService.BroadcastDeviceStatus(dto);

            _logger.LogInformation("Device status saved successfully for device: {DeviceId}", entity.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}