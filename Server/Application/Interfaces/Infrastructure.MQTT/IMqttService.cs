namespace Application.Interfaces.Infrastructure.MQTT;

public interface IMqttService
{
    Task SubscribeAsync(string topic);
}