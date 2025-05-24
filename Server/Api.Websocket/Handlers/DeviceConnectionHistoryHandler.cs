using System.Text.Json;
using Application.Interfaces;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Dtos.Ai;
using Application.Models.Dtos.MQTT;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

// klient skal sende noget, før vi kan sende data tilbage
// derfor requester klienten dette:
// { "eventType": "ClientRequestDeviceList" }
public class ClientRequestDeviceHistory : BaseDto
{
    public required string DeviceId { get; set; }
    
    // sæt default values
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// serveren sender dette tilbage til klienten
public class ServerResponseDeviceHistory : BaseDto
{
    public required PagedResult<DeviceConnectionHistoryDto> ConnectionData { get; set; }
}

public class DeviceConnectionHistoryHandler(IDeviceService deviceService) : BaseEventHandler<ClientRequestDeviceHistory>
{
    
    public override async Task Handle(ClientRequestDeviceHistory dto, IWebSocketConnection socket)
    {
        var historyData = await deviceService.GetDeviceHistory(dto.DeviceId, dto.PageNumber, dto.PageSize);
        
        var response = new WebsocketMessage<ServerResponseDeviceHistory>
        {
            Topic = WebsocketTopics.Device,
            eventType = "ServerResponseDeviceHistory",
            requestId = dto.requestId,
            Data = new ServerResponseDeviceHistory
            {
                ConnectionData = historyData
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}