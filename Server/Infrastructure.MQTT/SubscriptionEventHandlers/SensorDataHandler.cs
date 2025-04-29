using System.Text.Json;
using Application.Models.Dtos;
using Core.Domain.TestEntities;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers
{
    public class SensorDataHandler : IMqttMessageHandler
    {
        private readonly MyDbContextTestDocker _dbContext;
        private readonly ILogger<SensorDataHandler> _logger;

        public SensorDataHandler(MyDbContextTestDocker dbContext, ILogger<SensorDataHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public string TopicFilter => "AirQuality/Data";
        public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

        public void Handle(object? sender, OnMessageReceivedEventArgs args)
        {
            try
            {
                _logger.LogInformation("Handling message for topic: {Topic}", args.PublishMessage.Topic);
                var payload = args.PublishMessage.Payload;
                if (payload == null) return;

                var json = System.Text.Encoding.UTF8.GetString(payload);
                _logger.LogInformation("Received payload: {Payload}", json);

                var dto = JsonSerializer.Deserialize<SensorDataDto>(json);
                if (dto == null) return;

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
    }
}