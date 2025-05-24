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
public class ServerResponseList
{
    public required List<DeviceDto> DeviceList { get; set; }
}

/*
public class ExampleServerResponse : BaseDto
{
    public string SomethingTheServerSends { get; set; }
}*/

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
        
        var response = new WebsocketMessage<ServerResponseList>
        {
            Topic = WebsocketTopics.Device,
            eventType = WebsocketEvents.ServerResponseDeviceList,
            requestId = dto.requestId,
            Data = new ServerResponseList
            {
                DeviceList = result
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}