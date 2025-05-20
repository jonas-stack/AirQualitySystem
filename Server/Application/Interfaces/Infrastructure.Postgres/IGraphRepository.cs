using Application.Enums;
using Application.Models.Dtos.Graph;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IGraphRepository<T>
{
    Task<List<GraphModel<T>>> GetGraphDataAsync(GraphType graphType, TimePeriod timePeriod, DateTime? referenceDate = null);
}