namespace Application.Interfaces.Infrastructure.MQTT;

public interface IMqttService
{
    Task SetupMqttSubscriptionAsync(string topic);
}