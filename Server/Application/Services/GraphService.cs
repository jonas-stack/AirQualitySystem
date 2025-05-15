using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos.Graph;

namespace Application.Services;

public class GraphService(IConnectionManager connectionManager) : IGraphService
{
    private readonly Random _random = new();

    public async Task<GraphModel<int>> GetTotalMeasurementsAsync(string userId)
    {
        GraphModel<int> graphModel = new()
        {
            Name = "TotalMeasurements",
            Amount = _random.Next(0, 500),
            eventType = WebsocketEvents.GraphTotalMeasurement,
            topic = WebsocketTopics.Dashboard
        };
        
        connectionManager.BroadcastToTopic(WebsocketTopics.Dashboard, graphModel);
        return graphModel;
    }
}