using Application.Models.Dtos.Graph;
using Core.Domain.Entities;

namespace Application.Interfaces;

public interface IGraphService
{
    Task<GraphModel<int>> GetTotalMeasurementsAsync(SensorData sensorData);
}