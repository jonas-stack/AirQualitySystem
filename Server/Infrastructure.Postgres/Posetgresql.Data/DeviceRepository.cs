using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Mappers;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
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
    private readonly IDeviceConnectionHistoryMapper _deviceConnectionHistoryMapper;
    private readonly IDevicesMapper _devicesMapper;
    
    public DeviceRepository(MyDbContext dbContext, ILogger<SensorDataRepository> logger, IDeviceConnectionHistoryMapper deviceConnectionHistoryMapper, IDevicesMapper devicesMapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _deviceConnectionHistoryMapper = deviceConnectionHistoryMapper;
        _devicesMapper = devicesMapper;
    }

    public async Task<DeviceDto> GetDevice(string deviceId)
    {
        if (!Guid.TryParse(deviceId, out var guid))
            throw new ArgumentException($"'{deviceId}' is not a valid GUID");

        var result = await _dbContext.Devices.FirstOrDefaultAsync(d => d.DeviceId == guid);
        if (result == null)
            throw new ArgumentException($"'{deviceId}' does not exist");

        return _devicesMapper.MapToDto(result);
    }

    public async Task<PagedResult<DeviceConnectionHistoryDto>> GetDeviceConnectionHistoryAsync(string deviceId, int pageNumber, int pageSize)
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

        return new PagedResult<DeviceConnectionHistoryDto>
        {
            TotalCount = filtered.Count,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = paged.Select(_deviceConnectionHistoryMapper.MapToDto).ToList()
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

    public async Task AddDeviceConnectionHistory(Devices devices, Devices? existingDevice)
    {
        if (existingDevice != null)
        {
            if (devices.IsConnected == existingDevice.IsConnected)
                return;
        }
        
        var newDeviceHistory = new DeviceConnectionHistory()
        {
            DeviceId = devices.DeviceId,
            IsConnected = devices.IsConnected,
            LastSeen = devices.LastSeen
        };

        await _dbContext.AddAsync(newDeviceHistory);
        _logger.LogInformation("Device saved successfully: {DeviceId}", devices.DeviceId);

        await _dbContext.SaveChangesAsync();
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

            // opdater historien her efter vi har opdateret tiden
            await AddDeviceConnectionHistory(devices, existingDevice);

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

            _logger.LogInformation("Device saved successfully: {DeviceId}", devices.DeviceId);
            
            await _dbContext.SaveChangesAsync();
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
            var result = await _dbContext.Devices.OrderByDescending(d => d.LastSeen).ToListAsync();
            return result.Select(d => _devicesMapper.MapToDto(d)).ToList();
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
            var result = await _dbContext.Devices.FirstOrDefaultAsync() ?? throw new NullReferenceException("Device not found");
            return _devicesMapper.MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving status for device {DeviceId}", ex.Message);
            throw;
        }
    }
}