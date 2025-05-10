using System.Collections.Concurrent;
using Websocket.Client;

namespace Startup.Tests;

public class TestWsClient
{
    public TestWsClient()
    {
        var wsPort = Environment.GetEnvironmentVariable("PORT");
        if (string.IsNullOrEmpty(wsPort)) throw new Exception("Environment variable PORT is not set");
        WsClientId = Guid.NewGuid().ToString();
        var url = "ws://localhost:" + wsPort + "?id=" + WsClientId;
        var websocketUrl = new Uri(url);
        Console.WriteLine("Connecting to websocket at: " + websocketUrl);
        WsClient = new WebsocketClient(websocketUrl);

        WsClient.MessageReceived.Subscribe(msg => { ReceivedMessages.Enqueue(msg.Text); });
        WsClient.StartOrFail();
        Task.Delay(1000).GetAwaiter().GetResult();
    }

    public WebsocketClient WsClient { get; set; }
    public string WsClientId { get; set; }
    public ConcurrentQueue<string> ReceivedMessages { get; } = new();
}