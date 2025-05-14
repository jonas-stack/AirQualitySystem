using Application.Interfaces.Infrastructure.Postgres;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQTT.SubscriptionEventHandlers;

public class DeviceConnectionHandler : IMqttMessageHandler
{

    private readonly ILogger<DeviceConnectionHandler> _logger;
    private readonly DeviceConnectionTracker _connectionTracker;
    private readonly IDeviceConnectionRepository _deviceConnectionRepository;
    
    public DeviceConnectionHandler(
        ILogger<DeviceConnectionHandler> logger,
        DeviceConnectionTracker connectionTracker,
        IDeviceConnectionRepository deviceConnectionRepository)
    {
        _logger = logger;
        _connectionTracker = connectionTracker;
        _deviceConnectionRepository = deviceConnectionRepository;
    }

    public string TopicFilter => "airquality/status";
    public QualityOfService QoS => QualityOfService.AtMostOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs args)
    {
    }
}