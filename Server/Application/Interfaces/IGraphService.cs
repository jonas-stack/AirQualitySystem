using Application.Models.Dtos.Graph;

namespace Application.Interfaces;

public interface IGraphService
{
    Task<GraphModel<int>> GetTotalMeasurementsAsync(string userId);
}