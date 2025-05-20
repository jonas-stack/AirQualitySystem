using Application.Enums;
using Application.Models.Dtos.Graph;
using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IGraphService<T>
{
    Task  BroadcastTemperatureGraph(SensorData sensorData);
    
    Task BroadcastMeasurementsAsync(SensorData sensorData);
    Task<List<GraphModel<T>>> GetGraph(GraphType dtoGraphType, TimePeriod dtoTimePeriod);
}