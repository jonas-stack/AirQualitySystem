using System;
using System.Formats.Asn1;
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
            
            _logger.LogInformation("Created new MQTT subscriber");
        }

        public async Task SubscribeAsync(string topic)
        {
            await _client.SubscribeAsync(topic);
            _logger.LogInformation("Subscribed to topic: {Topic}", topic);
    
            // Remove existing handler first to avoid duplicates
            _client.OnMessageReceived -= HandleMessageReceived;
            _client.OnMessageReceived += HandleMessageReceived;
            
            _logger.LogInformation("Message handler registered");
        }

        private void HandleMessageReceived(object? sender, OnMessageReceivedEventArgs args)
        {
            byte[]? messagePayloadBytes = args.PublishMessage.Payload;

            string messageContent;
            if (messagePayloadBytes != null)
            {
                messageContent = System.Text.Encoding.UTF8.GetString(messagePayloadBytes);
            }
            else
            {
                messageContent = "(Empty payload)";
            }
            
            _logger.LogInformation("Recieved a message on topic: {Topic}", args.PublishMessage.Topic);
            _logger.LogInformation("Message content: {Content}", messageContent);

            IServiceScope scope = _serviceProvider.CreateScope();

            IEnumerable<IMqttMessageHandler> allHandlers =
                scope.ServiceProvider.GetRequiredService<IEnumerable<IMqttMessageHandler>>();
            
            // Check each handler to see if it should process this message
            bool foundMatchingHandler = false;
            foreach (IMqttMessageHandler handler in allHandlers)
            {
                string handlerName = handler.GetType().Name;
                string handlerTopic = handler.TopicFilter;
                string? messageTopic = args.PublishMessage.Topic;
                
                _logger.LogInformation("Checking if handler '{0}' can process message", handlerName);
                _logger.LogInformation("Handler topic: {0}, Message topic: {1}", handlerTopic, messageTopic);
                
                // Check if this handler's topic matches the message topic
                bool isTopicMatch = false;
                if (messageTopic != null)
                {
                    if (string.Equals(messageTopic, handlerTopic, StringComparison.OrdinalIgnoreCase))
                    {
                        isTopicMatch = true;
                    }
                }
                
                // If topics match, invoke the handler
                if (isTopicMatch)
                {
                    _logger.LogInformation("Found matching handler! Invoking handler for topic: {0}", handlerTopic);
                    handler.Handle(sender, args);
                    foundMatchingHandler = true;
                }
            }
            
            // Log warning if no handler was found
            if (!foundMatchingHandler)
            {
                _logger.LogWarning("No handler found for topic: {0}", args.PublishMessage.Topic);
            }
            
            // Dispose the scope when done
            scope.Dispose();
        }
    }
}