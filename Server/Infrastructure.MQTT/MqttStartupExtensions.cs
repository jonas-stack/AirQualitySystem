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

            var client = CreateMqttClient(optionsMonitor.CurrentValue, logger);
            ConfigureConnectionLifecycle(client, logger, lifetime);

            return client;
        });

        RegisterHandlers(services);
        services.AddSingleton<IMqttService, MqttSubscriber>();
        return services;
    }

    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var mqttClient = app.Services.GetRequiredService<HiveMQClient>();
        var logger = app.Services.GetRequiredService<ILogger<HiveMQClient>>();

        await SubscribeHandlers(app, mqttClient, logger);
        return app;
    }

    private static HiveMQClient CreateMqttClient(AppOptions options, ILogger logger)
    {
        if (string.IsNullOrEmpty(options.MQTT_USERNAME) || string.IsNullOrEmpty(options.MQTT_PASSWORD))
            throw new InvalidOperationException("MQTT_USERNAME or MQTT_PASSWORD is not set in the environment variables.");

        var clientOptions = new HiveMQClientOptionsBuilder()
            .WithWebSocketServer($"wss://{options.MQTT_BROKER_HOST}:8884/mqtt")
            .WithClientId($"AirQualitySystem_{Environment.MachineName}")  // Consistent ID
            .WithCleanStart(true)
            .WithKeepAlive(30)
            .WithAutomaticReconnect(true)
            .WithSessionExpiryInterval(600)  // 10-minute session expiry
            .WithUserName(options.MQTT_USERNAME)
            .WithPassword(options.MQTT_PASSWORD)
            .Build();

        return ConnectWithRetries(new HiveMQClient(clientOptions), logger);
    }

    private static HiveMQClient ConnectWithRetries(HiveMQClient client, ILogger logger, int maxRetries = 5)
    {
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("Connecting to MQTT broker (attempt {attempt}/{maxRetries})", attempt, maxRetries);
                client.ConnectAsync().GetAwaiter().GetResult();
                logger.LogInformation("Connected to MQTT broker");
                return client;
            }
            catch (HiveMQttClientException ex)
            {
                logger.LogError(ex, "Error connecting to MQTT broker on attempt {attempt}", attempt);
                if (attempt == maxRetries) throw;
                Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
        }
        throw new InvalidOperationException("Failed to connect to MQTT broker after retries.");
    }

    private static void ConfigureConnectionLifecycle(HiveMQClient client, ILogger logger, IHostApplicationLifetime lifetime)
    {
        client.OnDisconnectReceived += (sender, args) => logger.LogWarning("MQTT client disconnected");

        lifetime.ApplicationStopping.Register(async () =>
        {
            logger.LogInformation("Disconnecting from MQTT broker...");
            try
            {
                // Add timeout to avoid hanging on shutdown
                await Task.WhenAny(client.DisconnectAsync(), Task.Delay(3000));
                logger.LogInformation("Disconnected from MQTT broker.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error disconnecting MQTT client");
            }
        });
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var handlerTypes = GetHandlerTypes();
        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler);
        }
    }

    private static async Task SubscribeHandlers(WebApplication app, HiveMQClient mqttClient, ILogger logger)
    {
        // Get the MqttSubscriber service
        var mqttService = app.Services.GetRequiredService<IMqttService>();
        var handlerTypes = GetHandlerTypes();
    
        foreach (var handlerType in handlerTypes)
        {
            using var scope = app.Services.CreateScope();
            var handler = (IMqttMessageHandler)scope.ServiceProvider.GetRequiredService(handlerType);

            logger.LogInformation("Subscribing to topic: {topic} with QoS: {qos}", handler.TopicFilter, handler.QoS);
            // Use the service instead of client directly
            await mqttService.SubscribeAsync(handler.TopicFilter);
        }
    }

    private static IEnumerable<Type> GetHandlerTypes()
    {
        return typeof(IMqttMessageHandler).Assembly
            .GetTypes()
            .Where(t => typeof(IMqttMessageHandler).IsAssignableFrom(t) && !t.IsAbstract);
    }
}