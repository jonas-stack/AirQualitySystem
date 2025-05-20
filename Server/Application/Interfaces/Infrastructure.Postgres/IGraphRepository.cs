using Application.Enums;
using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IGraphRepository
{
    Task<List<MultiGraphEntity>> GetGraphDataAsync(TimePeriod timePeriod, DateTime? referenceDate = null);
}