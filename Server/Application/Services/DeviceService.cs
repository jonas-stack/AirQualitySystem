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
    private readonly IDeviceConnectionHistoryMapper _deviceConnectionHistoryMapper;
    
    public DeviceService(IDeviceRepository deviceRepository, IConnectionManager connectionManager, IDevicesMapper devicesMapper, IDeviceConnectionHistoryMapper deviceConnectionHistoryMapper)
    {
        _deviceRepository = deviceRepository;
        _connectionManager = connectionManager;
        _devicesMapper = devicesMapper;
        _deviceConnectionHistoryMapper = deviceConnectionHistoryMapper;
    }
    
    public async Task<List<DeviceDto>> GetAllDeviceStatus()
    {
        var devices = await _deviceRepository.GetAllDevices();
        var result = devices.Select(d => _devicesMapper.MapToDto(d)).OrderByDescending(d => d.LastSeen).ToList();
        
        return result;
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
        var result = await _deviceRepository.GetDeviceStatus();
        return _devicesMapper.MapToDto(result);
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

        DeviceConnectionHistory? nextEvent = null;

        foreach (var entry in pagedEntities.Items)
        {
            var unixTimestamp = ((DateTimeOffset)entry.LastSeen).ToUnixTimeSeconds();
        
            var dto = new DeviceConnectionHistoryDto
            {
                Id = entry.Id,
                DeviceId = entry.DeviceId.ToString(),
                IsConnected = entry.IsConnected,
                LastSeen = unixTimestamp,
                Duration = nextEvent != null
                    ? (long)(nextEvent.LastSeen - entry.LastSeen).TotalSeconds
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
}