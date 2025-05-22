namespace Application.Interfaces;

public interface IWebsocketNotifierService
{
    Task NotifyAsync(String topic, string message);
}