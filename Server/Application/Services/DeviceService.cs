using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Core.Domain.Entities;

namespace Application.Services;

public class DeviceService : IDeviceService {
    
    private readonly IDeviceRepository _deviceRepository;
    private readonly IConnectionManager _connectionManager;
    
    public DeviceService(IDeviceRepository deviceRepository, IConnectionManager connectionManager)
    {
        _deviceRepository = deviceRepository;
        _connectionManager = connectionManager;
    }
    
    public Task<List<DeviceDto>> GetAllDeviceStatus()
    {
        return _deviceRepository.GetAllDevices();
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
        
        return result;
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
        // thrower hvis ikke eksisterer, så ingen grund til at tjekke
        await _deviceRepository.GetDevice(deviceId);
        
        var pagedEntities = await _deviceRepository.GetDeviceConnectionHistoryAsync(deviceId, pageNumber, pageSize);
        return new PagedResult<DeviceConnectionHistoryDto>
        {
            TotalCount = pagedEntities.TotalCount,
            PageNumber = pagedEntities.PageNumber,
            PageSize = pagedEntities.PageSize,
            Items = pagedEntities.Items.Select(entity => new DeviceConnectionHistoryDto
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId.ToString(),
                LastSeen = entity.LastSeen.Ticks,
                IsConnected = entity.IsConnected,
            }).ToList()
        };
    }
}