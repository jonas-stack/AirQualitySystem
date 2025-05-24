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
        var result = devices.Select(d => _devicesMapper.MapToDto(d)).ToList();
        
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
        // thrower hvis ikke eksisterer, så ingen grund til at tjekke
        await _deviceRepository.GetDevice(deviceId);
        
        var pagedEntities = await _deviceRepository.GetDeviceConnectionHistoryAsync(deviceId, pageNumber, pageSize);
        return new PagedResult<DeviceConnectionHistoryDto>
        {
            TotalCount = pagedEntities.TotalCount,
            PageNumber = pagedEntities.PageNumber,
            PageSize = pagedEntities.PageSize,
            Items = pagedEntities.Items.Select(_deviceConnectionHistoryMapper.MapToDto).ToList()
        };
    }
}