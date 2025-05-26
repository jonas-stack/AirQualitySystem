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

public class SensorDataHandler : IMqttMessageHandler
{
    private readonly DeviceConnectionTracker _connectionTracker;
    private readonly IJsonDeserializer _deserializer;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<SensorDataHandler> _logger;
    private readonly ISensorDataMapper _mapper;
    private readonly ISensorDataRepository _sensorDataRepository;
    private readonly IDataValidator _validator;
    private readonly IAiCommunication _aiCommunication;
    private readonly IGraphService _graphService;

    public SensorDataHandler(
        ILogger<SensorDataHandler> logger,
        DeviceConnectionTracker connectionTracker,
        IDataValidator validator,
        ISensorDataMapper mapper,
        ISensorDataRepository sensorDataRepository, IJsonDeserializer deserializer, IDeviceRepository deviceRepository,
        IAiCommunication aiCommunicator)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _validator = validator;
        _mapper = mapper;
        _sensorDataRepository = sensorDataRepository;
        _deserializer = deserializer;
        _deviceRepository = deviceRepository;
        _aiCommunication = aiCommunicator;
    }

    public string TopicFilter => "AirQuality/Data";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public async Task HandleAsync(object? sender, OnMessageReceivedEventArgs args)
    {
        try
        {
            var payload = args.PublishMessage.Payload;
            if (payload == null) return;

            // Deserialize json payload fra mqtt til SensorDataDto
            var result = _deserializer.Deserialize<SensorDataDto>(payload);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to deserialize message: {Error}. Raw content: {Content}",
                    result.ErrorMessage, result.RawContent);
                return;
            }

            var dto = result.Data!;

            // Valider data for database lagring
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

            var entity = _mapper.MapToEntity(dto);

            // register ny device hvis den ikke findes ud fra deviceGuid
            if (!await _deviceRepository.DeviceExistsAsync(entity.DeviceId))
            {
                _logger.LogInformation("New device detected with ID {DeviceId}. Auto-registering.", dto.DeviceId);
                await _deviceRepository.RegisterNewDeviceAsync(entity.DeviceId, dto.DeviceId, entity.Timestamp);
            }

            // opdater device status
            _connectionTracker.UpdateDeviceStatus(entity.DeviceId, entity.Timestamp);

            // Send entity til repository for at gemme i database
            await _sensorDataRepository.SaveSensorDataAsync(entity);

            await _aiCommunication.BroadCastData();

            _logger.LogInformation("Data saved successfully for device: {DeviceId}", entity.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}