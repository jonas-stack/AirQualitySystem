using System.Text.Json;
using Application.Models.Dtos;
using Application.Services;
using Core.Domain.Entities;
using Core.Domain;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers
{
    public class SensorDataHandler : IMqttMessageHandler
    {
        private readonly MyDbContext _dbContext;
        private readonly ILogger<SensorDataHandler> _logger;
        private readonly DeviceConnectionTracker _connectionTracker;
        private readonly SensorDataValidator _validator;

        public SensorDataHandler(
            MyDbContext dbContext,
            ILogger<SensorDataHandler> logger,
            DeviceConnectionTracker connectionTracker,
            SensorDataValidator validator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionTracker = connectionTracker;
            _validator = validator;
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
                _connectionTracker.UpdateDeviceStatus(dto.DeviceId, dto.GetDateTime());

                // Validate data before saving using the dedicated validator
                if (!_validator.IsDataComplete(dto))
                {
                    _logger.LogWarning("Incomplete data received from device {DeviceId}. Skipping save.", dto.DeviceId);
                    return;
                }
                
                // Try to parse the device ID
                Guid deviceGuid;
                if (!Guid.TryParse(dto.DeviceId, out deviceGuid))
                {
                    // Create a deterministic GUID from the device name
                    deviceGuid = CreateDeterministicGuid(dto.DeviceId);
                    _logger.LogInformation("Created GUID {DeviceGuid} for device {DeviceId}", deviceGuid, dto.DeviceId);
                }

                var entity = new SensorData()
                {
                    Temperature = dto.Temperature,
                    Humidity = dto.Humidity,
                    AirQuality = dto.AirQuality,
                    Pm25 = dto.Pm25,
                    DeviceId = deviceGuid,
                    Timestamp = dto.GetLocalDateTime()
                };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
                _logger.LogInformation("Data saved successfully for device: {DeviceId}", entity.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
        
        private Guid CreateDeterministicGuid(string input)
        {
            // Use MD5 to create a deterministic hash from the input string
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
            
                // Convert hash to GUID format
                return new Guid(hashBytes);
            }
        }
    }
}