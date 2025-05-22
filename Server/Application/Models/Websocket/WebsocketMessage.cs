using WebSocketBoilerplate;

namespace Application.Models.Websocket;

public class WebsocketMessage<T> : BaseDto
{
    public required string Topic { get; set; }
    public required T Data { get; set; }
    public string requestId { get; set; } = string.Empty;
    public string eventType { get; set; } = string.Empty;
}