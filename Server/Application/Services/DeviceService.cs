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

    public Task<PagedResult<DeviceConnectionHistoryDto>> GetDeviceHistory(string dtoDeviceId, int dtoPageNumber, int dtoPageSize)
    {
        var historyData = _deviceRepository.GetDeviceHistory(string)
        throw new NotImplementedException();
    }
}