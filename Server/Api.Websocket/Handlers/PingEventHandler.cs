using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using Microsoft.Extensions.Logging;
using WebSocketBoilerplate;

namespace Api.Websocket.Handlers;

public class Ping : BaseDto { }
public class Pong : BaseDto { }

public class PingEventHandler(IConnectionManager connectionManager, ILogger<PingEventHandler> logger)
    : BaseEventHandler<Ping>
{
    public override Task Handle(Ping dto, IWebSocketConnection socket)
    {
        var clientId = connectionManager.GetClientIdFromSocket(socket);
        logger.LogInformation(clientId);
        socket.SendDto(new Pong());
        return Task.CompletedTask;
    }
}