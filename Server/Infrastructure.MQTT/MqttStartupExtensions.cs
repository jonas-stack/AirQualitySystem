using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Mappers;
using Application.Mappers;
using Application.Models;
using Application.Services;
using HiveMQtt.Client;
using HiveMQtt.Client.Exceptions;
using Infrastructure.MQTT.Services;
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
        // Create the client but don't connect yet
        services.AddSingleton<HiveMQClient>(serviceProvider => 
        {
            var options = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
            var logger = serviceProvider.GetRequiredService<ILogger<HiveMQClient>>();

            // Validation and options setup
            if (string.IsNullOrEmpty(options.MQTT_USERNAME) || string.IsNullOrEmpty(options.MQTT_PASSWORD))
                throw new InvalidOperationException("MQTT credentials not set");

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

            return new HiveMQClient(clientOptions);
        });

        // Register message handlers
        var handlerTypes = FindAllMessageHandlers();
        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType);
            services.AddScoped(typeof(IMqttMessageHandler), handlerType);
        }

        // Register other services
        services.AddSingleton<IMqttService, MqttSubscriber>();
        services.AddSingleton<DeviceConnectionTracker>();
        services.AddScoped<IDataValidator, DataValidator>();
        services.AddScoped<IJsonDeserializer, JsonDeserializer>();
        services.AddScoped<ISensorDataMapper, SensorDataMapper>();
        services.AddScoped<IDevicesMapper, DevicesMapper>();
        services.AddSingleton<IMqttPublisher, MqttPublisher>();

        return services;
    }

    // Method to set up MQTT during application startup
    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var mqttClient = app.Services.GetRequiredService<HiveMQClient>();
        var logger = app.Services.GetRequiredService<ILogger<HiveMQClient>>();
        var mqttService = app.Services.GetRequiredService<IMqttService>();

        // Connect with proper async retry
        await ConnectWithRetryAsync(mqttClient, logger);

        // Register disconnect handler
        mqttClient.OnDisconnectReceived += (sender, args) => {
            logger.LogWarning("MQTT client disconnected");
        };

        // Register application lifetime events
        var appLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStopping.Register(async () => {
            logger.LogWarning("Disconnecting from MQTT broker...");
            try {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await mqttClient.DisconnectAsync().WaitAsync(cts.Token);
                logger.LogInformation("Successfully disconnected from MQTT broker");
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error disconnecting MQTT client");
            }
        });

        // Setup subscriptions
        var handlerTypes = FindAllMessageHandlers();
        foreach (var handlerType in handlerTypes) {
            using var scope = app.Services.CreateScope();
            var handler = (IMqttMessageHandler)scope.ServiceProvider.GetRequiredService(handlerType);

            logger.LogInformation("Subscribing to topic: {topic} with QoS: {qos}",
                handler.TopicFilter, handler.QoS);

            await mqttService.SetupMqttSubscriptionAsync(handler.TopicFilter);
        }

        return app;
    }

    // Helper method to find all MQTT message handlers in the application
    private static List<Type> FindAllMessageHandlers()
    {
        var handlerType = typeof(IMqttMessageHandler);

        var handlers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => handlerType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        // Log discovered handlers
        Console.WriteLine($"Discovered {handlers.Count} message handlers:");
        foreach (var handler in handlers) 
            Console.WriteLine($"  - {handler.Name}");

        return handlers;
    }

    private static async Task ConnectWithRetryAsync(HiveMQClient client, ILogger logger)
    {
        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++) {
            try {
                logger.LogInformation("Connecting to MQTT broker (attempt {attempt}/{maxAttempts})",
                    attempt, maxAttempts);

                await client.ConnectAsync();
                logger.LogInformation("Connected to MQTT broker");
                return;
            }
            catch (HiveMQttClientException ex) {
                logger.LogError(ex, "Error connecting to MQTT broker on attempt {attempt}", attempt);

                if (attempt == maxAttempts)
                    throw new ApplicationException("Failed to connect to MQTT broker after maximum attempts", ex);

                // Wait with exponential backoff
                var waitSeconds = Math.Pow(2, attempt);
                await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
            }
        }
    }
}