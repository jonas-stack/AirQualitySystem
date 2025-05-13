using System.Text.Json;
using Application.Mappers;
using Application.Models.Dtos.MQTT;
using Application.Services;
using Application.Interfaces.Infrastructure.Postgres;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers
{
    public class SensorDataHandler : IMqttMessageHandler
    {
        private readonly ILogger<SensorDataHandler> _logger;
        private readonly DeviceConnectionTracker _connectionTracker;
        private readonly SensorDataValidator _validator;
        private readonly SensorDataMapper _mapper;
        private readonly IDeviceRepository _deviceRepository;

        public SensorDataHandler(
            ILogger<SensorDataHandler> logger,
            DeviceConnectionTracker connectionTracker,
            SensorDataValidator validator,
            SensorDataMapper mapper,
            IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _connectionTracker = connectionTracker;
            _validator = validator;
            _mapper = mapper;
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

                var json = System.Text.Encoding.UTF8.GetString(payload);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogInformation("Received empty message. Likely a retained message clear operation.");
                    return;
                }
        
                var dto = JsonSerializer.Deserialize<SensorDataDto>(json);
                if (dto == null) return;

                // Update device connection status
                var timestamp = _mapper.GetLocalDateTime(dto.TimestampUnix);
                _connectionTracker.UpdateDeviceStatus(dto.DeviceId, timestamp);

                // Validate data before saving using the dedicated validator
                if (!_validator.IsDataComplete(dto))
                {
                    _logger.LogWarning("Incomplete data received from device {DeviceId}. Skipping save.", dto.DeviceId);
                    return;
                }
                
                // Use mapper to create entity
                var entity = _mapper.MapToTestEntity(dto); //TODO CHANGE TO ACTUAL ENTITY FORM MAPPER
                
                // Use repository to save data
                _deviceRepository.SaveSensorData(entity);
                
                _logger.LogInformation("Data saved successfully for device: {DeviceId}", entity.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }
}