namespace Application.Models.Websocket;

public class WebsocketMessage<T>
{
    public required string Topic { get; set; }
    public required T Data { get; set; }
}