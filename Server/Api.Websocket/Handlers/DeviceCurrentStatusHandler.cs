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
public class ClientRequestDeviceStatus : BaseDto { }

// serveren sender dette tilbage til klienten
public class ServerResponseDeviceStatus : BaseDto
{
    public required DeviceDto DeviceStatus { get; set; }
}

public class DeviceCurrentStatusHandler : BaseEventHandler<ClientRequestDeviceStatus>
{
    private readonly IDeviceService _deviceService;
    
    public DeviceCurrentStatusHandler(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override async Task Handle(ClientRequestDeviceStatus dto, IWebSocketConnection socket)
    {
        var result = await _deviceService.GetDeviceStatus();
        
        var response = new WebsocketMessage<ServerResponseDeviceStatus>
        {
            Topic = WebsocketTopics.Device,
            Data = new ServerResponseDeviceStatus
            {
                DeviceStatus = result
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}