using Application.Interfaces;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Interfaces.Mappers;
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
        services.AddScoped<ISensorDataMapper, SensorDataMapper>();
        services.AddScoped<IDevicesMapper, DevicesMapper>();
        services.AddScoped<IAiCommunication, AiCommunicationService>();
        return services;
    }
}