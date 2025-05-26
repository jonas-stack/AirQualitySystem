using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Core.Domain.Entities;
using Infrastructure.Postgres.Helpers;
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

    public async Task<Devices> GetDevice(string deviceId)
    {
        if (!Guid.TryParse(deviceId, out var guid))
            throw new ArgumentException($"'{deviceId}' is not a valid GUID");

        var result = await _dbContext.Devices.FirstOrDefaultAsync(d => d.DeviceId == guid);
        if (result == null)
            throw new ArgumentException($"'{deviceId}' does not exist");

        return result;
    }

    public async Task<PagedResult<DeviceConnectionHistory>> GetDeviceConnectionHistoryAsync(string deviceId, int pageNumber, int pageSize)
    {
        if (!Guid.TryParse(deviceId, out var guid))
            throw new ArgumentException($"'{deviceId}' is not a valid GUID");

        var fullHistory = await _dbContext.DeviceConnectionHistory
            .Where(sd => sd.DeviceId == guid)
            .OrderByDescending(sd => sd.LastSeen)
            .Take(500)
            .ToListAsync();

        var filtered = new List<DeviceConnectionHistory>();
        bool? lastConnectionState = null;

        foreach (var entry in fullHistory)
        {
            if (entry.IsConnected == lastConnectionState)
                continue;

            filtered.Add(entry);
            lastConnectionState = entry.IsConnected;
        }

        var paged = filtered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<DeviceConnectionHistory>
        {
            TotalCount = filtered.Count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = paged
        };
    }

    public DeviceStatsDto GetStats()
    {
        var allTimeMeasurements = _dbContext.SensorData.Count();
        var connectedDevices = _dbContext.Devices.Count(d => d.IsConnected);
        
        var last24Hours = DateTime.Now.AddHours(-24);
        var disconnectionsLast24Hours = _dbContext.DeviceConnectionHistory.Count(d => d.LastSeen < last24Hours && !d.IsConnected);

        return new DeviceStatsDto
        {
            AllTimeMeasurements = allTimeMeasurements,
            ConnectedDevices = connectedDevices,
            DisconnectionsLast24Hours = disconnectionsLast24Hours
        };
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
    
    public async Task<List<Devices>> GetAllDevices()
    {
        try
        {
            return await _dbContext.Devices.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all device status");
            throw;
        }
    }

    public async Task<Devices> GetDeviceStatus()
    {
        try
        {
            return await _dbContext.Devices.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Device not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving status for device {DeviceId}", ex.Message);
            throw;
        }
    }
}