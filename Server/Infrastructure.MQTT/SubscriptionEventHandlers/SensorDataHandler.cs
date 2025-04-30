using System.Text.Json;
using Application.Models.Dtos;
using Core.Domain.TestEntities;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Infrastructure.MQTT.Services;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers
{
    public class SensorDataHandler : IMqttMessageHandler
    {
        private readonly MyDbContextTestDocker _dbContext;
        private readonly ILogger<SensorDataHandler> _logger;
        private readonly DeviceConnectionTracker _connectionTracker;

        public SensorDataHandler(MyDbContextTestDocker dbContext, ILogger<SensorDataHandler> logger,
            DeviceConnectionTracker connectionTracker)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionTracker = connectionTracker;
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
                var dto = JsonSerializer.Deserialize<SensorDataDto>(json);
                if (dto == null) return;

                // Update device connection status
                _connectionTracker.UpdateDeviceStatus(dto.DeviceId, dto.GetDateTime());

                // Validate data before saving
                if (!IsDataComplete(dto))
                {
                    _logger.LogWarning("Incomplete data received from device {DeviceId}. Skipping save.", dto.DeviceId);
                    return;
                }

                var entity = new Sensordata
                {
                    Temperature = dto.Temperature,
                    Humidity = dto.Humidity,
                    Airquality = dto.AirQuality,
                    Pm25 = dto.Pm25,
                    Deviceid = dto.DeviceId,
                    Timestamp = dto.GetDateTime()
                };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
                _logger.LogInformation("Data saved successfully for device: {DeviceId}", entity.Deviceid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }

        private bool IsDataComplete(SensorDataDto dto)
        {
            // Define what "complete data" means - these are examples
            if (dto.Temperature <= -100 || dto.Temperature >= 100) return false;
            if (dto.Humidity < 0 || dto.Humidity > 100) return false;
            if (dto.AirQuality <= 0) return false;
            if (dto.Pm25 < 0) return false;
            if (string.IsNullOrEmpty(dto.DeviceId)) return false;

            // Check if message timestamp is reasonable (not too old, not future)
            var messageTime = dto.GetDateTime();
            if (messageTime < DateTime.UtcNow.AddHours(-1) || messageTime > DateTime.UtcNow.AddMinutes(5))
                return false;

            return true;
        }
    }
}