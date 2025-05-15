using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos.Graph;
using Core.Domain.Entities;

namespace Application.Services;

public class GraphService(IConnectionManager connectionManager) : IGraphService
{
    public async Task<GraphModel<int>> GetTotalMeasurementsAsync(SensorData sensorData)
    {
        String currentTime = DateTime.Now.ToString("HH:mm");
        int tempInt = (int)Math.Round(sensorData.Temperature);

        GraphModel<int> graphModel = new()
        {
            Identifier = currentTime,
            Amount = tempInt,
            eventType = WebsocketEvents.GraphTotalMeasurement,
            topic = WebsocketTopics.Dashboard
        };
        
        connectionManager.BroadcastToTopic(WebsocketTopics.Dashboard, graphModel);
        return graphModel;
    }
}