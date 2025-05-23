using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class WebsocketNotifierService(IConnectionManager connectionManager) : IWebsocketNotifierService
{
    public async Task NotifyAsync(string topic, string message)
    {
       await connectionManager.BroadcastToTopic(topic, message);
    }
}