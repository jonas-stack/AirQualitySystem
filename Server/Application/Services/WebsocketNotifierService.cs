using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;

namespace Application.Services;

public class WebsocketNotifierService(IConnectionManager connectionManager) : IWebsocketNotifierService
{
    public async Task NotifyAsync(string topic, string message)
    {
        await connectionManager.BroadcastToTopic(topic, message);
        await Task.CompletedTask;
    }
}