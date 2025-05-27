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
    public required DeviceStatsDto stats { get; set; }
}

public class DeviceRequestStatsHandler(IDeviceService deviceService) : BaseEventHandler<ClientRequestDeviceStats>
{
    public override async Task Handle(ClientRequestDeviceStats dto, IWebSocketConnection socket)
    {
        var result = deviceService.GetDeviceStats();
            
        var response = new ServerResponseDeviceStats
        {
            eventType = nameof(ServerResponseDeviceStats),
            requestId = dto.requestId,
            stats = result
        };
        
        await socket.Send(JsonSerializer.Serialize(response));
    }
}