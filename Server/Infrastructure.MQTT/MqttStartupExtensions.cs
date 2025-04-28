using Application.Interfaces.Infrastructure.MQTT;
using Application.Models;
using HiveMQtt.Client;
using HiveMQtt.Client.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.MQTT;

public static class MqttExtensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<HiveMQClient>(sp =>
        {
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<AppOptions>>();
            var logger = sp.GetRequiredService<ILogger<HiveMQClient>>();
            var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();

            var username = optionsMonitor.CurrentValue.MQTT_USERNAME;
            var password = optionsMonitor.CurrentValue.MQTT_PASSWORD;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException(
                    "MQTT_USERNAME or MQTT_PASSWORD is not set in the environment variables.");

            var options = new HiveMQClientOptionsBuilder()
                .WithWebSocketServer($"wss://{optionsMonitor.CurrentValue.MQTT_BROKER_HOST}:8884/mqtt")
                .WithClientId($"myClientId_{Guid.NewGuid()}")
                .WithCleanStart(true)
                .WithKeepAlive(30)
                .WithAutomaticReconnect(true)
                .WithUserName(username)
                .WithPassword(password)
                .Build();

            var client = new HiveMQClient(options);

            client.OnDisconnectReceived += (sender, args) => logger.LogWarning("MQTT client disconnected");

            const int maxRetries = 5;
            for (var attempt = 1; attempt <= maxRetries; attempt++)
                try
                {
                    logger.LogInformation("Connecting to MQTT broker (attempt {attempt}/{maxRetries})", attempt,
                        maxRetries);
                    client.ConnectAsync().GetAwaiter().GetResult();
                    logger.LogInformation("Connected to MQTT broker");
                    break;
                }
                catch (HiveMQttClientException ex)
                {
                    logger.LogError(ex, "Error connecting to MQTT broker on attempt {attempt}", attempt);
                    if (attempt == maxRetries) throw;
                    Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }

            // Register shutdown logic
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Disconnecting from MQTT broker...");
                client.DisconnectAsync().GetAwaiter().GetResult();
                logger.LogInformation("Disconnected from MQTT broker.");
            });

            return client;
        });

        var handlers = typeof(IMqttMessageHandler).Assembly
            .GetTypes()
            .Where(t => typeof(IMqttMessageHandler).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var handler in handlers) services.AddScoped(handler);

        services.AddSingleton<IMqttService, MqttSubscriber>();
        return services;
    }

    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var mqttClient = app.Services.GetRequiredService<HiveMQClient>();
        var logger = app.Services.GetRequiredService<ILogger<HiveMQClient>>();

        var handlers = typeof(IMqttMessageHandler).Assembly
            .GetTypes()
            .Where(t => typeof(IMqttMessageHandler).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var handlerType in handlers)
        {
            using var scope = app.Services.CreateScope();
            var handler = (IMqttMessageHandler)scope.ServiceProvider.GetRequiredService(handlerType);

            logger.LogInformation("Subscribing to topic: {topic} with QoS: {qos}", handler.TopicFilter, handler.QoS);

            // Pass the topic and QoS separately
            await mqttClient.SubscribeAsync(handler.TopicFilter, handler.QoS);
        }

        return app;
    }
}