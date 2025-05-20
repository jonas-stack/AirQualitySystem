using System.Text;
using Application.Interfaces.Infrastructure.MQTT;
using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.Services;

public class MqttSubscriber : IMqttService, IAsyncDisposable
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
    public ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing MQTT subscriber");
        DisconnectAsync().GetAwaiter().GetResult();
        return ValueTask.CompletedTask;
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

    private async void HandleMessageReceived(object? sender, OnMessageReceivedEventArgs args)
    {
        var messageContent = args.PublishMessage.Payload != null
            ? Encoding.UTF8.GetString(args.PublishMessage.Payload)
            : "(Empty payload)";

        _logger.LogInformation("Received a message on topic: {Topic}", args.PublishMessage.Topic);
        _logger.LogInformation("Message content: {Content}", messageContent);

        using (var scope = _serviceProvider.CreateScope())
        {
            var allHandlers =
                scope.ServiceProvider.GetRequiredService<IEnumerable<IMqttMessageHandler>>();

            var foundMatchingHandler = false;
            foreach (var handler in allHandlers)
            {
                var handlerTopic = handler.TopicFilter;
                var messageTopic = args.PublishMessage.Topic;

                var isTopicMatch = messageTopic != null &&
                                   string.Equals(messageTopic, handlerTopic, StringComparison.OrdinalIgnoreCase);

                if (isTopicMatch)
                {
                    _logger.LogDebug("Found handler {HandlerName} for topic: {Topic}",
                        handler.GetType().Name, handlerTopic);
                    await handler.HandleAsync(sender, args);
                    foundMatchingHandler = true;
                }
            }

            if (!foundMatchingHandler) _logger.LogWarning("No handler found for topic: {0}", args.PublishMessage.Topic);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_client != null)
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