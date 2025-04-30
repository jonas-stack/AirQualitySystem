using System;
using System.Threading.Tasks;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using Application.Interfaces.Infrastructure.MQTT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT
{
    public class MqttSubscriber : IMqttService
    {
        private readonly HiveMQClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MqttSubscriber> _logger;

        public MqttSubscriber(HiveMQClient client,IServiceProvider serviceProvider, ILogger<MqttSubscriber> logger)
        {
            _client = client;
            _serviceProvider = serviceProvider;
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
            var payload = args.PublishMessage.Payload;
            string payloadString = payload != null 
                ? System.Text.Encoding.UTF8.GetString(payload) 
                : "(empty payload)";

            _logger.LogInformation("Message received on topic: {Topic}, content: {Content}",
                args.PublishMessage.Topic, payloadString);

            // Create a fresh scope for each message
            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IMqttMessageHandler>>();

            foreach (var handler in handlers)
            {
                _logger.LogInformation("Checking handler {HandlerType} for topic {HandlerTopic} against message topic {MessageTopic}", 
                    handler.GetType().Name, handler.TopicFilter, args.PublishMessage.Topic);
    
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