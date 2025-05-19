using System.Text.Json;
using Application.Interfaces;
using Application.Models;
using Application.Models.Dtos.Ai;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

public class ClientRequestDeviceList : BaseDto { }

public class ServerResponseList : BaseDto
{
    public List<DeviceDto> DeviceList { get; set; }
}

/*
public class ExampleServerResponse : BaseDto
{
    public string SomethingTheServerSends { get; set; }
}*/

public class DeviceConnectionHandler : BaseEventHandler<ClientRequestDeviceList>
{
    private readonly IDeviceService _deviceService;
    
    public DeviceConnectionHandler(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override async Task Handle(ClientRequestDeviceList dto, IWebSocketConnection socket)
    {
        var result = await _deviceService.GetAllDeviceStatus();
        
        var response = new WebsocketMessage<ServerResponseList>
        {
            Topic = WebsocketTopics.Device,
            Data = new ServerResponseList
            {
                DeviceList = result
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}