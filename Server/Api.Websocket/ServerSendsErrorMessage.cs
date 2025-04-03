using WebSocketBoilerplate;

namespace Api.Websocket;

public class ServerSendsErrorMessage : BaseDto
{
    public string Message { get; set; }
}