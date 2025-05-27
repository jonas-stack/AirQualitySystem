namespace Application.Interfaces.Infrastructure.MQTT;

public interface IMqttPublisher
{
    Task Publish(object dto, string topic);
}