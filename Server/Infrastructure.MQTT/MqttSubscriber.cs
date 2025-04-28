using System;
using System.Threading.Tasks;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using Application.Interfaces.Infrastructure.MQTT;

namespace Infrastructure.MQTT
{
    public class MqttSubscriber : IMqttService
    {
        private readonly HiveMQClient _client;
        private readonly IEnumerable<IMqttMessageHandler> _handlers;

        public MqttSubscriber(HiveMQClient client, IEnumerable<IMqttMessageHandler> handlers)
        {
            _client = client;
            _handlers = handlers;
        }

        public async Task SubscribeAsync(string topic)
        {
            await _client.SubscribeAsync(topic);
            Console.WriteLine($"Subscribed to topic: {topic}");

            _client.OnMessageReceived += (sender, args) =>
            {
                Console.WriteLine($"Message received on topic: {args.PublishMessage.Topic}");
                foreach (var handler in _handlers)
                {
                    if (args.PublishMessage.Topic == handler.TopicFilter)
                    {
                        handler.Handle(sender, args);
                    }
                }
            };
        }
    }
}