using Application.Enums;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.Graph;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class GraphRepository<T> : IGraphRepository<T> {
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;

    public GraphRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public Task<List<GraphModel<T>>> GetGraphDataAsync(GraphType graphType, TimePeriod timePeriod, DateTime? referenceDate = null)
    {
        throw new NotImplementedException();
    }
}