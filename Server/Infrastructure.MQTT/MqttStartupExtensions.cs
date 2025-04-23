using Application.Interfaces.Infrastructure.MQTT;
using Application.Models;
using HiveMQtt.Client;
using HiveMQtt.Client.Exceptions;
using Infrastructure.MQTT.SubscriptionEventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HiveMQtt.Client.Options;

namespace Infrastructure.MQTT;

public static class MqttStartupExtensions
{
    public static IServiceCollection AddMqttServices(this IServiceCollection services)
    {
        services.AddSingleton<HiveMQClient>(sp =>
        {
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<AppOptions>>();
            var logger = sp.GetRequiredService<ILogger<HiveMQClient>>();

            var clientOptions = new HiveMQClientOptionsBuilder()
                .WithWebSocketServer(
                    $"wss://{optionsMonitor.CurrentValue.MQTT_BROKER_HOST}:8884/mqtt") // Using WSS (secure WebSocket)
                .WithClientId($"myClientId_{Guid.NewGuid()}")
                .WithCleanStart(true)
                .WithKeepAlive(30)
                .WithAutomaticReconnect(true)
                .WithMaximumPacketSize(1024)
                .WithReceiveMaximum(100)
                .WithSessionExpiryInterval(3600)
                .WithUserName(optionsMonitor.CurrentValue.MQTT_USERNAME)
                .WithPassword(optionsMonitor.CurrentValue.MQTT_PASSWORD)
                .WithRequestProblemInformation(true)
                .WithRequestResponseInformation(true)
                .WithAllowInvalidBrokerCertificates(true)
                .Build();

            var client = new HiveMQClient(clientOptions);

            client.OnDisconnectReceived += (sender, args) =>
                logger.LogWarning("MQTT client disconnected");

            client.ConnectAsync().GetAwaiter().GetResult();
            return client;
        });

        services.AddScoped<SensorDataHandler>();
        services.AddScoped<IMqttService, MqttSubscriber>();

        return services;
    }

    public static async Task SubscribeToTopics(this IServiceProvider serviceProvider, params string[] topics)
    {
        using var scope = serviceProvider.CreateScope();
        var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<HiveMQClient>>();

        foreach (var topic in topics)
        {
            try
            {
                logger.LogInformation("Subscribing to topic: {topic}", topic);
                await mqttService.SubscribeAsync(null!, topic);
                logger.LogInformation("Successfully subscribed to topic: {topic}", topic);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to subscribe to topic: {topic}", topic);
            }
        }
    }
}