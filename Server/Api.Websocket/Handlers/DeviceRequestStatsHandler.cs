using System.Text.Json;
using Application.Interfaces;
using Application.Models.Dtos;
using Application.Models.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

public class ClientRequestDeviceStats : BaseDto { }

// serveren sender dette tilbage til klienten
public class ServerResponseDeviceStats : BaseDto
{
    public required DeviceStatsDto Stats { get; set; }
}

public class DeviceRequestStatsHandler(IDeviceService deviceService) : BaseEventHandler<ClientRequestDeviceStats>
{
    public override async Task Handle(ClientRequestDeviceStats dto, IWebSocketConnection socket)
    {
        var result = deviceService.GetDeviceStats();
            
        var response = new WebsocketMessage<ServerResponseDeviceStats>
        {
            Topic = WebsocketTopics.Device,
            eventType = "ServerResponseDeviceStats",
            requestId = dto.requestId,
            Data = new ServerResponseDeviceStats
            {
                Stats = result,
            }
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}