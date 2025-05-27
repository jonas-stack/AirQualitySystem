using Application.Enums;
using Application.Models.Websocket.Graph;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IGraphRepository
{
    Task<List<FlexibleGraphData>> GetGraphDataAsync(TimePeriod timePeriod, DateTime? referenceDate = null);
}