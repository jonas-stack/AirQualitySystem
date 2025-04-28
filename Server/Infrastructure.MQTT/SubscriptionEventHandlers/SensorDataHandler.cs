using System.Text.Json;
using Core.Domain.Entities;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.MQTT.SubscriptionEventHandlers
{
    public class SensorDataHandler : IMqttMessageHandler
    {
        private readonly MyDbContextTestDocker _dbContext;

        public SensorDataHandler(MyDbContextTestDocker dbContext)
        {
            _dbContext = dbContext;
        }

        public string TopicFilter => "airquality/data";
        public QualityOfService QoS => QualityOfService.AtLeastOnceDelivery;

        public void Handle(object? sender, OnMessageReceivedEventArgs args)
        {
            try
            {
                var payload = args.PublishMessage.Payload;
                if (payload == null)
                {
                    Console.WriteLine("Payload is null.");
                    return;
                }
                var json = System.Text.Encoding.UTF8.GetString(payload);
                Console.WriteLine($"Received payload: {json}");

                var entity = JsonSerializer.Deserialize<SensorData>(json);
                if (entity == null)
                {
                    Console.WriteLine("Failed to deserialize payload.");
                    return;
                }

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
                Console.WriteLine($"Data saved for device: {entity.DeviceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
}