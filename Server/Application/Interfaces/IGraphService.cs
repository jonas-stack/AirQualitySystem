using Application.Models.Dtos.Graph;
using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IGraphService
{
    Task  BroadcastTemperatureGraph(SensorData sensorData);
    
    Task BroadcastMeasurementsAsync(SensorData sensorData);
}