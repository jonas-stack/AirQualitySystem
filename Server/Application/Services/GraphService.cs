using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos.Graph;
using Application.Models.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class GraphService<T>(IConnectionManager connectionManager) : IGraphService<T>
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
        };
        
        var response = new WebsocketMessage<GraphModel<int>>
        {
            Topic = WebsocketTopics.DeviceStatus,
            Data = graphModel
        };
        
        await connectionManager.BroadcastToTopic(WebsocketTopics.Dashboard, response);
    }
    
    public Task<List<GraphModel<T>>> GetGraph(GraphType dtoGraphType, TimePeriod dtoTimePeriod)
    {
        throw new NotImplementedException();
    }
}