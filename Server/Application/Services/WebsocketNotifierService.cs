using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class WebsocketNotifierService(IConnectionManager connectionManager, IGraphService graphService) : IWebsocketNotifierService
{
    private readonly Random _random = new();
    public async Task NotifyAsync(string topic, string message)
    {
       SensorData sensordata = new SensorData();
       sensordata.Timestamp = DateTime.Now;
       sensordata.Id = 1;
       sensordata.Temperature = _random.Next(10, 25);
        
        await graphService.GetTotalMeasurementsAsync(sensordata);
        await Task.CompletedTask;
    }
}