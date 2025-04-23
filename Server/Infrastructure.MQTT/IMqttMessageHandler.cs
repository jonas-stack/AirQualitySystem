namespace Infrastructure.MQTT;

using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;

public interface IMqttMessageHandler
{
    string TopicFilter { get; }
    QualityOfService QoS { get; }
    void Handle(object? sender, OnMessageReceivedEventArgs args);
}