using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Web;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketBoilerplate;

namespace Api.Websocket;

public static class Extensions
{
    public static IServiceCollection RegisterWebsocketApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        var assembly = typeof(Extensions).Assembly;
        services.InjectEventHandlers(assembly);
        return services;
    }

    public static async Task<WebApplication> ConfigureWebsocketApi(this WebApplication app, int wsPort = 8181)
    {
        app.UseRouting();
        var port = GetAvailablePort(wsPort);
        Environment.SetEnvironmentVariable("PORT", port.ToString());
        var url = $"ws://0.0.0.0:{port}";
        var logger = app.Services.GetRequiredService<ILogger<NonStaticWsExtensionClassForLogger>>();
        logger.LogInformation("WS running on url: " + url);
        var server = new WebSocketServer(url);

        Action<IWebSocketConnection> config = ws =>
        {
            var queryString = ws.ConnectionInfo.Path.Split('?').Length > 1
                ? ws.ConnectionInfo.Path.Split('?')[1]
                : "";

            var id = HttpUtility.ParseQueryString(queryString)["id"] ??
                     throw new Exception("Please specify ID query param for websocket connection");
            
            using var scope = app.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IConnectionManager>();
            
            ws.OnOpen = () => manager.OnOpen(ws, id);
            ws.OnClose = () => manager.OnClose(ws, id);
            ws.OnError = ex => ws.SendDto(new ServerSendsErrorMessage { Message = ex.Message });
            ws.OnMessage = async message =>
            {
                try
                {
                    await app.CallEventHandler(ws, message);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error in handling message: {message}", message);
                    
                    // Validate that the message is valid JSON; otherwise, the entire program will crash.
                    
                    var baseDto = JsonSerializer.Deserialize<BaseDto>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new BaseDto
                    {
                        eventType = nameof(ServerSendsErrorMessage)
                    };
                    var resp = new ServerSendsErrorMessage
                        { Message = e.Message, requestId = baseDto.requestId ?? "" };
                    ws.SendDto(resp);
                }
            };
        };

        server.Start(config);
        return app;
    }

    private static int GetAvailablePort(int startPort)
    {
        var port = startPort;
        var isPortAvailable = false;

        do
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Loopback, port);
                tcpListener.Start();
                tcpListener.Stop();
                isPortAvailable = true;
            }
            catch (SocketException)
            {
                port++;
            }
        } while (!isPortAvailable);

        return port;
    }
}

public class NonStaticWsExtensionClassForLogger;