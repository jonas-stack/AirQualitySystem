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

// klient skal sende noget, før vi kan sende data tilbage
// derfor requester klienten dette:
// { "eventType": "ClientRequestDeviceList" }
public class ClientRequestDeviceStatus : BaseDto { }

// serveren sender dette tilbage til klienten
public class ServerResponseDeviceStatus : BaseDto
{
    [JsonPropertyName("deviceStatus")]
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
        
        var response = new ServerResponseDeviceStatus
        {
            eventType = nameof(ServerResponseDeviceStatus),
            requestId = dto.requestId,
            DeviceStatus = result
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}