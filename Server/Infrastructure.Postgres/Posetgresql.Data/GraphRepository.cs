using System.Globalization;
using Application.Enums;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class GraphRepository : IGraphRepository {
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;

    public GraphRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<List<MultiGraphEntity>> GetGraphDataAsync(TimePeriod timePeriod, DateTime? referenceDate = null)
    {
        // start med at finde ud af hvornår vi skal starte med at kigge
        // f.eks. en dag siden, en uge siden osv.
        DateTime startDate = DetermineStartDate(timePeriod, referenceDate);
        
        // opret base query, ved at kigge påå start datoen
        var query = CreateBaseQuery(startDate);
        
        // opret queries og returner
        return await AggregateDataByTimePeriod(query, timePeriod);
    }
    
    private IQueryable<SensorData> CreateBaseQuery(DateTime startDate)
    {
        var query = _dbContext.Set<SensorData>()
            .Where(d => d.Timestamp >= startDate);
            
        return query;
    }
    
    private DateTime DetermineStartDate(TimePeriod timePeriod, DateTime? referenceDate = null)
    {
        var refDate = referenceDate ?? DateTime.UtcNow;

        // Convert to 'Unspecified' so EF Core won't complain when talking to timestamp without time zone
        refDate = DateTime.SpecifyKind(refDate, DateTimeKind.Unspecified);

        return timePeriod switch
        {
            TimePeriod.Hourly => refDate.AddDays(-1),
            TimePeriod.Daily => refDate.AddDays(-7),
            TimePeriod.Weekly => refDate.AddDays(-30),
            TimePeriod.Monthly => refDate.AddDays(-365),
            _ => throw new ArgumentOutOfRangeException(nameof(timePeriod))
        };
    }
    
    private async Task<List<MultiGraphEntity>> AggregateDataByTimePeriod(IQueryable<SensorData> query, TimePeriod timePeriod, int? maxResults = null) 
    {
        switch (timePeriod)
        {
            case TimePeriod.Hourly:
                var hourlyQuery = query
                    .GroupBy(d => d.Timestamp.Hour)
                    .Select(g => new
                    {
                        TimeKey = g.Key,
                        Temperature = g.Average(d => d.Temperature),
                        Humidity = g.Average(d => d.Humidity),
                        AirQuality = g.Average(d => d.AirQuality),
                        Pm25 = g.Average(d => d.Pm25)
                    });

                if (maxResults.HasValue)
                    hourlyQuery = hourlyQuery.Take(maxResults.Value);

                var hourly = await hourlyQuery.ToListAsync();

                return hourly.Select(d => new MultiGraphEntity
                {
                    Timestamp = $"{d.TimeKey:00}:00",
                    Values = BuildValuesDictionary(d)
                }).ToList();

            case TimePeriod.Daily:
                var dailyQuery = query
                    .GroupBy(d => d.Timestamp.Date)
                    .Select(g => new
                    {
                        TimeKey = g.Key,
                        DayOfWeek = g.Key.DayOfWeek,
                        Temperature = g.Average(d => d.Temperature),
                        Humidity = g.Average(d => d.Humidity),
                        AirQuality = g.Average(d => d.AirQuality),
                        Pm25 = g.Average(d => d.Pm25)
                    });

                if (maxResults.HasValue)
                    dailyQuery = dailyQuery.Take(maxResults.Value);

                var daily = await dailyQuery.ToListAsync();

                return daily.Select(d => new MultiGraphEntity
                {
                    Timestamp = d.DayOfWeek.ToString().Substring(0, 3),
                    Values = BuildValuesDictionary(d)
                }).ToList();

            case TimePeriod.Weekly:
                var weeklyRaw = await query.ToListAsync();

                var weeklyGrouped = weeklyRaw
                    .GroupBy(d =>
                    {
                        var week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            d.Timestamp,
                            CalendarWeekRule.FirstDay,
                            DayOfWeek.Monday);
                        return new
                        {
                            Year = d.Timestamp.Year,
                            Week = week
                        };
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Week = g.Key.Week,
                        Temperature = g.Average(d => d.Temperature),
                        Humidity = g.Average(d => d.Humidity),
                        AirQuality = g.Average(d => d.AirQuality),
                        Pm25 = g.Average(d => d.Pm25)
                    });

                if (maxResults.HasValue)
                    weeklyGrouped = weeklyGrouped.Take(maxResults.Value);

                return weeklyGrouped.Select(d => new MultiGraphEntity
                {
                    Timestamp = $"Week {d.Week}",
                    Values = BuildValuesDictionary(d)
                }).ToList();

            case TimePeriod.Monthly:
                var monthlyRaw = await query.ToListAsync();

                var monthlyGrouped = monthlyRaw
                    .GroupBy(d => new
                    {
                        Year = d.Timestamp.Year,
                        Month = d.Timestamp.Month,
                        Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            d.Timestamp,
                            CalendarWeekRule.FirstDay,
                            DayOfWeek.Monday) % 5 + 1
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Week = g.Key.Week,
                        Temperature = g.Average(d => d.Temperature),
                        Humidity = g.Average(d => d.Humidity),
                        AirQuality = g.Average(d => d.AirQuality),
                        Pm25 = g.Average(d => d.Pm25)
                    });

                if (maxResults.HasValue)
                    monthlyGrouped = monthlyGrouped.Take(maxResults.Value);

                return monthlyGrouped.Select(d => new MultiGraphEntity
                {
                    Timestamp = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Month)} - Week {d.Week}",
                    Values = BuildValuesDictionary(d)
                }).ToList();

            default:
                throw new ArgumentOutOfRangeException(nameof(timePeriod));
        }
    }


    private Dictionary<string, double> BuildValuesDictionary(dynamic d)
    {
        var dict = new Dictionary<string, double>();

        if (d is IDictionary<string, object> expando)
        {
            foreach (var kvp in expando)
            {
                if (kvp.Value != null && double.TryParse(kvp.Value.ToString(), out double dValue))
                {
                    dict[kvp.Key.ToLower()] = dValue;
                }
            }
        }
        else
        {
            // fallback to reflection (see above)
            var props = d.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(d);
                double dValue = 0.0;

                if (value != null && double.TryParse(value.ToString(), out dValue))
                {
                    dict[prop.Name.ToLower()] = dValue;
                }
            }
        }

        return dict;
    }



}