using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;

namespace Application.Interfaces.Infrastructure.MQTT;

public interface IMqttMessageHandler
{
    string TopicFilter { get; }
    QualityOfService QoS { get; }
    Task HandleAsync(object? sender, OnMessageReceivedEventArgs args);
}