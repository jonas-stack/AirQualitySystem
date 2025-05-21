using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class WebsocketNotifierService(IConnectionManager connectionManager, IGraphService graphService) : IWebsocketNotifierService
{
    public async Task NotifyAsync(string topic, string message)
    {
        SensorData sensorData = new SensorData()
        {
            Temperature = -10
        }; 
        
        await graphService.BroadcastTemperatureGraph(sensorData);
      // await connectionManager.BroadcastToTopic(topic, message);
    }
}