using System.Text.Json;
using Application.Interfaces;
using Application.Models;
using Application.Models.Dtos.Ai;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// klient skal sende noget, før vi kan sende data tilbage
// derfor requester klienten dette:
// { "eventType": "ClientRequestDeviceList" }
public class ClientRequestDeviceList : BaseDto { }

// serveren sender dette tilbage til klienten
public class ServerResponseDeviceList : BaseDto
{
    public required List<DeviceDto> deviceList { get; set; }
}

public class GetAllDevicesHandler : BaseEventHandler<ClientRequestDeviceList>
{
    private readonly IDeviceService _deviceService;
    
    public GetAllDevicesHandler(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override async Task Handle(ClientRequestDeviceList dto, IWebSocketConnection socket)
    {
        var result = await _deviceService.GetAllDeviceStatus();
        
        var response = new ServerResponseDeviceList
        {
            eventType = nameof(ServerResponseDeviceList),
            requestId = dto.requestId,
            deviceList = result.ToList()
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}