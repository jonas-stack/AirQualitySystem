using System.Text.Json;
using System.Text.Json.Serialization;
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
    [JsonPropertyName("deviceId")]
    public required string DeviceId { get; set; }
    
    // sæt default values
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;
    
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 50;
}

// serveren sender dette tilbage til klienten
public class ServerResponseDeviceHistory : BaseDto
{
    [JsonPropertyName("connectionData")]
    public required PagedResult<DeviceConnectionHistoryDto> ConnectionData { get; set; }
}

public class DeviceConnectionHistoryHandler(IDeviceService deviceService) : BaseEventHandler<ClientRequestDeviceHistory>
{
    public override async Task Handle(ClientRequestDeviceHistory dto, IWebSocketConnection socket)
    {
        var historyData = await deviceService.GetDeviceHistory(dto.DeviceId, dto.PageNumber, dto.PageSize);
        
        var response = new ServerResponseDeviceHistory
        {
            eventType = nameof(ServerResponseDeviceHistory),
            requestId = dto.requestId,
            ConnectionData = historyData
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}