using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Interfaces.Mappers;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class DeviceService : IDeviceService {
    
    private readonly IDeviceRepository _deviceRepository;
    private readonly IConnectionManager _connectionManager;
    private readonly IDevicesMapper _devicesMapper;
    
    public DeviceService(IDeviceRepository deviceRepository, IConnectionManager connectionManager, IDevicesMapper devicesMapper)
    {
        _deviceRepository = deviceRepository;
        _connectionManager = connectionManager;
        _devicesMapper = devicesMapper;
    }
    
    public async Task<List<DeviceDto>> GetAllDeviceStatus()
    {
        return await _deviceRepository.GetAllDevices();
    }

    public async Task BroadcastDeviceStatus(DeviceDto dto)
    {
        var response = new WebsocketMessage<DeviceDto>
        {
            Topic = WebsocketTopics.DeviceStatus,
            Data = dto
        };
        
        await _connectionManager.BroadcastToTopic(WebsocketTopics.DeviceStatus, response);
    }

    public async Task<DeviceDto> GetDeviceStatus()
    {
        return await _deviceRepository.GetDeviceStatus();
    }

    public async Task BroadcastData(DeviceDto entity)
    {
        var response = new WebsocketMessage<DeviceDto>
        {
            Topic = WebsocketTopics.Device,
            eventType = WebsocketEvents.BroadcastDeviceConnectionUpdate,
            Data = entity
        };
        
        await _connectionManager.BroadcastToTopic(WebsocketTopics.Device, response);
    }

    public async Task<PagedResult<DeviceConnectionHistoryDto>> GetDeviceHistory(string deviceId, int pageNumber, int pageSize)
    {
        await _deviceRepository.GetDevice(deviceId);

        var pagedEntities = await _deviceRepository.GetDeviceConnectionHistoryAsync(deviceId, pageNumber, pageSize);
        var items = new List<DeviceConnectionHistoryDto>();

        DeviceConnectionHistoryDto? nextEvent = null;

        foreach (var entry in pagedEntities.Items)
        {
            var dateTime = new DateTime(entry.LastSeen);
            var unixTimestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            
            var dto = new DeviceConnectionHistoryDto
            {
                Id = entry.Id,
                DeviceId = entry.DeviceId.ToString(),
                IsConnected = entry.IsConnected,
                LastSeen = unixTimestamp,
                Duration = nextEvent != null
                    ? ((DateTimeOffset)new DateTime(nextEvent.LastSeen)).ToUnixTimeSeconds() - unixTimestamp
                    : null
            };

            nextEvent = entry;
            items.Add(dto);
        }

        return new PagedResult<DeviceConnectionHistoryDto>
        {
            TotalCount = pagedEntities.TotalCount,
            PageNumber = pagedEntities.PageNumber,
            PageSize = pagedEntities.PageSize,
            Items = items
        };
    }

    public DeviceStatsDto GetDeviceStats()
    {
        return _deviceRepository.GetStats();
    }
}