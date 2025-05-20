using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos.Graph;
using Application.Models.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class GraphService(IConnectionManager connectionManager) : IGraphService
{
    public async Task BroadcastMeasurementsAsync(SensorData sensorData)
    {
        await BroadcastTemperatureGraph(sensorData);
    }

    public async Task BroadcastTemperatureGraph(SensorData sensorData)
    {
        String currentTime = DateTime.Now.ToString("HH:mm");
        int tempInt = (int)Math.Round(sensorData.Temperature);

        GraphModel<int> graphModel = new()
        {
            Identifier = currentTime,
            Amount = tempInt,
            EventType = WebsocketEvents.GraphTotalMeasurement,
            Topic = WebsocketTopics.Dashboard
        };
        
        await connectionManager.BroadcastToTopic(WebsocketTopics.Dashboard, graphModel);
    }
}