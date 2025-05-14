using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models.Dtos.Graph;
using Application.Models.Websocket;

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
            eventType = GraphEnum.TOTAL_MEASUREMENTS.GetTopicName()
        };
        connectionManager.BroadcastToTopic("measurements", graphModel);
        return graphModel;
    }
}