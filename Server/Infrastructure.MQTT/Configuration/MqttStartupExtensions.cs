using Application.Interfaces.Infrastructure.MQTT;
using Application.Models;
using Application.Services;
using HiveMQtt.Client;
using HiveMQtt.Client.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.MQTT.Configuration;

public static class MqttExtensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<HiveMQClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
            var logger = serviceProvider.GetRequiredService<ILogger<HiveMQClient>>();
            var appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();


            if (string.IsNullOrEmpty(options.MQTT_USERNAME) || string.IsNullOrEmpty(options.MQTT_PASSWORD))
                throw new InvalidOperationException(
                    "MQTT_USERNAME or MQTT_PASSWORD is not set in the environment variables.");

            // Set up client options
            var clientOptions = new HiveMQClientOptionsBuilder()
                .WithWebSocketServer($"wss://{options.MQTT_BROKER_HOST}:8884/mqtt")
                .WithClientId($"AirQualitySystem_{Environment.MachineName}")
                .WithCleanStart(true)
                .WithKeepAlive(30)
                .WithAutomaticReconnect(true)
                .WithSessionExpiryInterval(600)
                .WithUserName(options.MQTT_USERNAME)
                .WithPassword(options.MQTT_PASSWORD)
                .Build();

            // Create the client
            var client = new HiveMQClient(clientOptions);

            // Try connecting with retries
            for (var attempt = 1; attempt <= 5; attempt++)
                try
                {
                    logger.LogInformation("Connecting to MQTT broker (attempt {attempt}/5)", attempt);
                    client.ConnectAsync().GetAwaiter().GetResult();
                    logger.LogInformation("Connected to MQTT broker");
                    break;
                }
                catch (HiveMQttClientException ex)
                {
                    logger.LogError(ex, "Error connecting to MQTT broker on attempt {attempt}", attempt);
                    if (attempt == 5) throw;

                    // Wait before next attempt with exponential backoff
                    var waitSeconds = (int)Math.Pow(2, attempt);
                    Thread.Sleep(TimeSpan.FromSeconds(waitSeconds));
                }

            // Set up disconnect handler
            client.OnDisconnectReceived += (sender, args) => { logger.LogWarning("MQTT client disconnected"); };

            // Set up graceful shutdown
            appLifetime.ApplicationStopping.Register(async () =>
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

            return client;
        });

        // Find and register all MQTT message handlers
        var handlerTypes = FindAllMessageHandlers();
        foreach (var handlerType in handlerTypes)
        {
            // Register each handler with DI
            services.AddScoped(handlerType);
            services.AddScoped(typeof(IMqttMessageHandler), handlerType);
        }

        // Register other MQTT services
        services.AddSingleton<IMqttService, MqttSubscriber>();
        services.AddSingleton<DeviceConnectionTracker>();
        services.AddScoped<IDataValidator, DataValidator>();
        services.AddScoped<IMqttMessageDeserializer, MqttMessageDeserializer>();

        return services;
    }

    // Method to set up MQTT during application startup
    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var mqttClient = app.Services.GetRequiredService<HiveMQClient>();
        var logger = app.Services.GetRequiredService<ILogger<HiveMQClient>>();
        var mqttService = app.Services.GetRequiredService<IMqttService>();

        // Find all message handlers we need to subscribe to
        var handlerTypes = FindAllMessageHandlers();

        // Subscribe to each handler's topic
        foreach (var handlerType in handlerTypes)
        {
            // Create a scope for this handler
            using var scope = app.Services.CreateScope();

            // Get the handler instance
            var handler = (IMqttMessageHandler)scope.ServiceProvider.GetRequiredService(handlerType);

            // Log subscription attempt
            logger.LogInformation("Subscribing to topic: {topic} with QoS: {qos}",
                handler.TopicFilter, handler.QoS);

            // Subscribe to the topic
            await mqttService.SetupMqttSubscriptionAsync(handler.TopicFilter);
        }

        return app;
    }

    // Helper method to find all MQTT message handlers in the application
    private static List<Type> FindAllMessageHandlers()
    {
        // Get all types from all loaded assemblies
        var handlers = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        foreach (var type in assembly.GetTypes())
            // Check if this type implements our handler interface and isn't abstract
            if (typeof(IMqttMessageHandler).IsAssignableFrom(type) && !type.IsAbstract)
                handlers.Add(type);

        // Log what we found
        Console.WriteLine($"Discovered {handlers.Count} message handlers:");
        foreach (var handler in handlers) Console.WriteLine($"  - {handler.Name}");

        return handlers;
    }
}