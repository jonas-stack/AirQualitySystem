﻿using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Posetgresql.Data;

public class DeviceRepository : IDeviceRepository
{
    private readonly MyDbContext _dbContext;
    private readonly ILogger<SensorDataRepository> _logger;
    
    public DeviceRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task SaveDevicesAsync(Devices devices)
    {
        try
        {
            var existingDevice = await _dbContext.Devices.FindAsync(devices.DeviceId);

            if (!devices.IsConnected)
            {
                devices.LastSeen = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
            }
            else
            {
                devices.LastSeen = new DateTime(devices.LastSeen.Ticks, DateTimeKind.Unspecified);
            }

            if (existingDevice != null)
            {
                existingDevice.DeviceName = devices.DeviceName;
                existingDevice.IsConnected = devices.IsConnected;
                existingDevice.LastSeen = devices.LastSeen;
                _dbContext.Update(existingDevice);
            }
            else
            {
                _dbContext.Add(devices);
            }

            var newDeviceHistory = new DeviceConnectionHistory()
            {
                DeviceId = devices.DeviceId,
                IsConnected = devices.IsConnected,
                LastSeen = devices.LastSeen
            };

            _dbContext.Add(newDeviceHistory);

            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("Device saved successfully: {DeviceId}", devices.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving device {DeviceId}", devices.DeviceId);
            throw;
        }
    }

    public async Task<bool> DeviceExistsAsync(Guid deviceId)
    {
        try
        {
            return await _dbContext.Devices.AnyAsync(d => d.DeviceId == deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if device {DeviceId} exists", deviceId);
            throw;
        }
    }

    public async Task RegisterNewDeviceAsync(Guid deviceId, string deviceName, DateTime lastSeen)
    {
        try
        {
            var newDevice = new Devices
            {
                DeviceId = deviceId,
                DeviceName = deviceName,
                IsConnected = true,
                LastSeen = new DateTime(lastSeen.Ticks, DateTimeKind.Unspecified)
            };

            await SaveDevicesAsync(newDevice);
            _logger.LogInformation("Device {DeviceId} automatically registered", deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering new device {DeviceId}", deviceId);
            throw;
        }
    }
    
    public async Task<List<DeviceDto>> GetAllDevices()
    {
        try
        {
            return await _dbContext.Devices
                .Select(d => new DeviceDto
                {
                    DeviceGuid = d.DeviceId.ToString(),
                    DeviceName = d.DeviceName,
                    IsConnected = d.IsConnected,
                    LastSeen = d.LastSeen.Ticks
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all device status");
            throw;
        }
    }

    public async Task<DeviceDto> GetDeviceStatus()
    {
        try
        {
            return await _dbContext.Devices
                .Select(d => new DeviceDto
                {
                    DeviceGuid = d.DeviceId.ToString(),
                    IsConnected = d.IsConnected,
                })
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Device not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving status for device {DeviceId}", ex.Message);
            throw;
        }
    }
}