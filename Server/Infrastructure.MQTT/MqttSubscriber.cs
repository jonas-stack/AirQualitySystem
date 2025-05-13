using System.Text;
using Application.Interfaces.Infrastructure.MQTT;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT;

public class MqttSubscriber : IMqttService, IDisposable
{
    private readonly HiveMQClient _client;
    private readonly ILogger<MqttSubscriber> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MqttSubscriber(HiveMQClient client, IServiceProvider serviceProvider, ILogger<MqttSubscriber> logger)
    {
        _client = client;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("Created new MQTT subscriber");
    }

    public async Task SetupMqttSubscriptionAsync(string topic)
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
        string messageContent = args.PublishMessage.Payload != null
            ? Encoding.UTF8.GetString(args.PublishMessage.Payload)
            : "(Empty payload)";

        _logger.LogInformation("Received a message on topic: {Topic}", args.PublishMessage.Topic);
        _logger.LogInformation("Message content: {Content}", messageContent);

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            IEnumerable<IMqttMessageHandler> allHandlers =
                scope.ServiceProvider.GetRequiredService<IEnumerable<IMqttMessageHandler>>();

            bool foundMatchingHandler = false;
            foreach (IMqttMessageHandler handler in allHandlers)
            {
                string handlerTopic = handler.TopicFilter;
                string? messageTopic = args.PublishMessage.Topic;

                bool isTopicMatch = messageTopic != null &&
                                    string.Equals(messageTopic, handlerTopic, StringComparison.OrdinalIgnoreCase);

                if (isTopicMatch)
                {
                    _logger.LogDebug("Found handler {HandlerName} for topic: {Topic}",
                        handler.GetType().Name, handlerTopic);
                    handler.Handle(sender, args);
                    foundMatchingHandler = true;
                }
            }

            if (!foundMatchingHandler)
            {
                _logger.LogWarning("No handler found for topic: {0}", args.PublishMessage.Topic);
            }
        }
    }

    public void Dispose() //dispose mqtt session when disconnectiong from broker.
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
    
    public async Task DisconnectAsync()
    {
        if (_client != null)
        {
            try
            {
                _logger.LogInformation("Disconnecting from MQTT broker");
                await _client.DisconnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting from MQTT broker");
            }
            finally
            {
                _client.OnMessageReceived -= HandleMessageReceived;
            }
        }
    }
}