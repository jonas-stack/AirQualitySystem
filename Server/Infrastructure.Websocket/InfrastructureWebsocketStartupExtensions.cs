using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using WebSocketBoilerplate;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionManager, WebSocketConnectionManager>();
        return services;
    }
}