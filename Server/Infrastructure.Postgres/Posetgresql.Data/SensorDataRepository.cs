﻿using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Mappers;
using Application.Mappers;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;
using Infrastructure.Postgres.Helpers;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;
    private readonly ISensorDataMapper _sensorDataMapper;

    public SensorDataRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger, ISensorDataMapper sensorDataMapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sensorDataMapper = sensorDataMapper;
    }

    public async Task SaveSensorDataAsync(SensorData sensorData)
    {
        try
        {
            await _dbContext.AddAsync(sensorData);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Sensor data saved successfully for device: {DeviceId}", sensorData.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving sensor data for device {DeviceId}", sensorData.DeviceId);
            throw; // Re-throw to allow higher layers to handle or report the error
        }
    }

    public async Task<PagedResult<SensorDataDto>> GetSensorDataForDeviceAsync(string deviceId, int pageNumber = 1, int pageSize = 50)
    {
        if (!Guid.TryParse(deviceId, out var guid))
            throw new ArgumentException($"'{deviceId}' is not a valid GUID");

        var query = _dbContext.SensorData
            .Where(sd => sd.DeviceId == guid)
            .OrderByDescending(sd => sd.Timestamp);

        return await PaginationHelper.PaginateAsync<SensorData, SensorDataDto>(
            query,
            pageNumber,
            pageSize,
            _sensorDataMapper.MapToDto);
    }
}