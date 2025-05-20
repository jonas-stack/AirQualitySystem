using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Websocket;
using Application.Models.Websocket.Graph;
using Core.Domain.Entities;
using MultiGraphEntity = Core.Domain.Entities.MultiGraphEntity;

namespace Application.Services;

public class GraphService(IConnectionManager connectionManager, IGraphRepository graphRepository) : IGraphService
{
    public async Task BroadcastMeasurementsAsync(SensorData sensorData)
    {
        await BroadcastTemperatureGraph(sensorData);
    }

    public async Task BroadcastTemperatureGraph(SensorData sensorData)
    {
        String currentTime = DateTime.Now.ToString("HH:mm");
        
        GraphModel graphModel = new()
        {
            Label = currentTime,
            Amount = Math.Round(sensorData.Temperature, 2)
        };
        
        var response = new WebsocketMessage<GraphModel>
        {
            Topic = WebsocketTopics.DeviceStatus,
            Data = graphModel
        };
        
        await connectionManager.BroadcastToTopic(WebsocketTopics.Dashboard, response);
    }
    
    public async Task<List<FlexibleGraphData>> GetFlexibleGraphDataAsync(List<string> dataKeys, TimePeriod timePeriod, DateTime? referenceDate = null)
    {
        var rawData = await graphRepository.GetGraphDataAsync(timePeriod, referenceDate);

        var formatted = rawData.Select(item =>
        {
            var dataPoints = new Dictionary<string, double>();

            foreach (var key in dataKeys)
            {
                if (item.Values.TryGetValue(key, out var value))
                {
                    dataPoints[key] = Math.Round(value, 2);
                }
            }

            return new FlexibleGraphData
            {
                Time = item.Timestamp,
                DataPoints = dataPoints
            };
        }).ToList();
        return formatted;
    }

}