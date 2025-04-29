using System;
using System.Threading.Tasks;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using Application.Interfaces.Infrastructure.MQTT;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT
{
    public class MqttSubscriber : IMqttService
    {
        private readonly HiveMQClient _client;
        private readonly IEnumerable<IMqttMessageHandler> _handlers;
        private readonly ILogger<MqttSubscriber> _logger;

        public MqttSubscriber(HiveMQClient client, IEnumerable<IMqttMessageHandler> handlers, ILogger<MqttSubscriber> logger)
        {
            _client = client;
            _handlers = handlers;
            _logger = logger;
        }

        public async Task SubscribeAsync(string topic)
        {
            await _client.SubscribeAsync(topic);
            _logger.LogInformation("Subscribed to topic: {Topic}", topic);
    
            // Remove existing handler first to avoid duplicates
            _client.OnMessageReceived -= HandleMessageReceived;
            _client.OnMessageReceived += HandleMessageReceived;
        }

        private void HandleMessageReceived(object? sender, OnMessageReceivedEventArgs args)
        {
            _logger.LogInformation("Message received on topic: {Topic}", args.PublishMessage.Topic);
    
            foreach (var handler in _handlers)
            {
                if (args.PublishMessage.Topic != null && 
                    string.Equals(args.PublishMessage.Topic, handler.TopicFilter, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Invoking handler for topic: {Topic}", handler.TopicFilter);
                    handler.Handle(sender, args);
                }
            }
        }
    }
}