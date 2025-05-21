using System.Globalization;
using Application.Enums;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                var hourlyGrouped = query
                    .GroupBy(d => new { d.Timestamp.Date, d.Timestamp.Hour })
                    .Select(g => new
                    {
                        TimeKey = new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, 0, 0),
                        Temperature = g.Average(d => d.Temperature),
                        Humidity = g.Average(d => d.Humidity),
                        AirQuality = g.Average(d => d.AirQuality),
                        Pm25 = g.Average(d => d.Pm25)
                    });

                hourlyGrouped = hourlyGrouped.OrderBy(g => g.TimeKey);
                
                if (maxResults.HasValue)
                    hourlyGrouped = hourlyGrouped.Take(maxResults.Value);

                var hourly = await hourlyGrouped.ToListAsync();

                return hourly.Select(d => new MultiGraphEntity
                {
                    Timestamp = d.TimeKey.ToString("HH:mm"),
                    Values = BuildValuesDictionary(d)
                }).ToList();

            case TimePeriod.Daily:
                var today = DateTime.Today;
                
                // denne mandag
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

                var dailyGrouped = query
                    .Where(d => d.Timestamp.Date >= startOfWeek && d.Timestamp.Date <= today)
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

                dailyGrouped = dailyGrouped.OrderBy(g => g.TimeKey);

                if (maxResults.HasValue)
                    dailyGrouped = dailyGrouped.Take(maxResults.Value);
                
                var daily = await dailyGrouped.ToListAsync();

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
                        var week = ISOWeek.GetWeekOfYear(d.Timestamp);
                        var year = ISOWeek.GetYear(d.Timestamp);
                        return new { Year = year, Week = week };
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

                weeklyGrouped = weeklyGrouped.OrderBy(g => g.Week);

                if (maxResults.HasValue)
                    weeklyGrouped = weeklyGrouped.Take(maxResults.Value);

                return weeklyGrouped.Select(d => new MultiGraphEntity
                {
                    Timestamp = $"Week {d.Week}",
                    Values = BuildValuesDictionary(d)
                }).ToList();

            case TimePeriod.Monthly:
                var monthlyRaw = await query
                    .Where(d => d.Timestamp >= query.FirstOrDefault()!.Timestamp)
                    .ToListAsync();

                var monthlyGrouped = monthlyRaw
                    .GroupBy(d => d.Timestamp.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Temperature = g.Average(d => d.Temperature),
                        Humidity = g.Average(d => d.Humidity),
                        AirQuality = g.Average(d => d.AirQuality),
                        Pm25 = g.Average(d => d.Pm25)
                    });

                monthlyGrouped = monthlyGrouped.OrderBy(g => g.Month);

                if (maxResults.HasValue)
                    monthlyGrouped = monthlyGrouped.Take(maxResults.Value);

                return monthlyGrouped
                    .Select(d => new MultiGraphEntity
                    {
                        Timestamp = CultureInfo.CurrentCulture.DateTimeFormat
                            .GetMonthName(d.Month)
                            .Substring(0, 1).ToUpper() + CultureInfo.CurrentCulture.DateTimeFormat
                            .GetMonthName(d.Month).Substring(1), // lidt bøvl men første uppercase månede
                        Values = BuildValuesDictionary(d)
                    })
                    .ToList();
            
            default:
                throw new ArgumentOutOfRangeException(nameof(timePeriod));
        }
    }

    private Dictionary<string, double> BuildValuesDictionary(dynamic d)
    {
        var dict = new Dictionary<string, double>();

        var props = d.GetType().GetProperties();
        foreach (var prop in props)
        {
            var value = prop.GetValue(d);

            if (value is string s && double.TryParse(s, out double dValue))
            {
                dict[prop.Name.ToLower()] = dValue;
            }
            else if (value is double doub)
            {
                dict[prop.Name.ToLower()] = doub;
            }
            else if (value is float f)
            {
                dict[prop.Name.ToLower()] = f;
            }
            else if (value is decimal dec)
            {
                dict[prop.Name.ToLower()] = (double)dec;
            }
        }

        return dict;
    }
}