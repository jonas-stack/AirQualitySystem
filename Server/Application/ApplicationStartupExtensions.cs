using Application.Interfaces;
using Application.Interfaces.EntityServices;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Mappers;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationStartupExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IWebsocketSubscriptionService, WebsocketSubscriptionService>();
        services.AddScoped<IDataValidator, DataValidator>();
        services.AddScoped<SensorDataMapper>();
        services.AddScoped<DevicesMapper>();
        return services;
    }
}