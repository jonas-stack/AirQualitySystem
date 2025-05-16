using Application.Interfaces.Infrastructure.Ai;
using Application.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Ai;

public static class AiStartupExstensions
{
    public static IServiceCollection RegisterAiService(this IServiceCollection services, AppOptions appOptions)
    {
        services.AddHttpClient<IAiService, AiApiService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30000);
            client.BaseAddress = new Uri(appOptions.AiApiBaseUrl);
        });
        return services;
    }
}