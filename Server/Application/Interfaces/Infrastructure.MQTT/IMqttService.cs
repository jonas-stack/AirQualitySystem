namespace Application.Interfaces.Infrastructure.MQTT;

public interface IMqttService
{
    Task SubscribeAsync(object dto, string topic);
}