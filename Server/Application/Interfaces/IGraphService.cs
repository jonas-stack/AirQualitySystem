using Application.Enums;
using Application.Models.Websocket.Graph;
using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IGraphService
{
    Task  BroadcastTemperatureGraph(SensorData sensorData);
    
    Task BroadcastMeasurementsAsync(SensorData sensorData);

   // Task<List<MultiGraphEntity>> GetGraphDataAsync(GraphType graphType, TimePeriod timePeriod, DateTime? referenceDate = null);

   //data keys er f.eks.
   // ["temperature", "humidity", "pm25"]
   Task<List<FlexibleGraphData>> GetFlexibleGraphDataAsync(List<string> dataKeys, TimePeriod timePeriod,
       DateTime? referenceDate = null);

}