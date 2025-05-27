using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Models;
using Application.Models.Dtos.Ai;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

public class DeviceUpdateIntervalHandler : BaseEventHandler<DeviceIntervalUpdateDto>
{
    private readonly IDeviceService _deviceService;
    
    public DeviceUpdateIntervalHandler(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override async Task Handle(DeviceIntervalUpdateDto dto, IWebSocketConnection socket)
    {
        await _deviceService.UpdateDeviceInterval(dto);
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}