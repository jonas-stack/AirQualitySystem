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
public class ClientRequestSensorData : BaseDto
{
    public required string SensorId { get; set; }
}

// serveren sender dette tilbage til klienten
public class ServerResponseSensorData : BaseDto
{
    public required List<SensorDataDto> SensorData { get; set; }
}

/*
public class ExampleServerResponse : BaseDto
{
    public string SomethingTheServerSends { get; set; }
}*/

public class DeviceSensorDataHandler : BaseEventHandler<ClientRequestSensorData>
{
    private readonly IDeviceService _deviceService;
    
    public DeviceSensorDataHandler(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override async Task Handle(ClientRequestSensorData dto, IWebSocketConnection socket)
    {
        var response = new WebsocketMessage<ServerResponseSensorData>
        {
            Topic = WebsocketTopics.Device,
            eventType = WebsocketEvents.ServerResponseDeviceList,
            requestId = dto.requestId,
            Data = new ServerResponseSensorData
            {
                SensorData = null
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}