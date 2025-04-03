using Application.Interfaces;
using Application.Interfaces.EntityServices;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationStartupExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        
        services.AddScoped<IEntityService, EntityService>();
        return services;
    }
}