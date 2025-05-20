using Api.Rest;
using Api.Websocket;
using Application;
using Application.Interfaces.Infrastructure.MQTT;
using Application.Models;
using Infrastructure.Ai;
using Infrastructure.MQTT;
using Infrastructure.Postgres;
using Infrastructure.Websocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NLog;
using NSwag.Generation;
using Startup.Documentation;
using Startup.Proxy;

//using Startup.Documentation; //TODO figure out what this is in alex fullstack2025 project
//using Startup.Proxy; //TODO figure out what this is in alex fullstack2025 project

namespace Startup;

public class Program
{
    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();
        
        // Add environment variables and JSON configuration
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        // this is for cheking the enviornemt, depending on usersecret (see id in startup.csproj)
        var environment = builder.Environment.EnvironmentName;
        Console.WriteLine("Current environment: " + environment);

        if (builder.Environment.IsProduction())
        {
            Console.WriteLine("Running in production mode");
        }
        else
        {
            Console.WriteLine("Running in development mode");
        }
        
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        await ConfigureMiddleware(app);
        await app.RunAsync();
    }
    
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var appOptions = services.AddAppOptions(configuration);
        
        services.RegisterApplicationServices();

        services.AddDataSourceAndRepositories();
        services.AddWebsocketInfrastructure();
        services.RegisterAiService(appOptions);

        services.RegisterWebsocketApiServices();
        services.RegisterRestApiServices();
        services.AddOpenApiDocument(conf =>
        {
            conf.DocumentProcessors.Add(new AddAllDerivedTypesProcessor());
            conf.DocumentProcessors.Add(new AddStringConstantsProcessor());
            conf.DocumentProcessors.Add(new AddWebsocketTopicsEnumProcessor());
            conf.DocumentProcessors.Add(new AddWebsocketEventsProcessor());

        });
        services.AddSingleton<IProxyConfig, ProxyConfig>();
        
        services.AddDbContext<Infrastructure.Postgres.Scaffolding.MyDbContext>(options =>
                    options.UseNpgsql(appOptions.DbConnectionString));
        
        services.AddScoped<Seeder>();

        services.RegisterMqttInfrastructure();


    }

    public static async Task ConfigureMiddleware(WebApplication app)
    {
        var appOptions = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;

        using (var scope = app.Services.CreateScope())
        {
            if (appOptions.Seed)
                await scope.ServiceProvider.GetRequiredService<Seeder>().Seed();
        }


        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{appOptions.REST_PORT}");
        app.Services.GetRequiredService<IProxyConfig>()
            .StartProxyServer(appOptions.PORT, appOptions.REST_PORT, appOptions.WS_PORT);

        app.ConfigureRestApi();
        await app.ConfigureWebsocketApi(appOptions.WS_PORT);
        await app.ConfigureMqtt();


        app.MapGet("Acceptance", () => "Accepted");

        app.UseOpenApi(conf => { conf.Path = "openapi/v1.json"; });

        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>().GenerateAsync("v1");
        var json = document.ToJson();
        await File.WriteAllTextAsync("openapi.json", json);
        
        //TODO uncomment the line below. it's a function to generate a TypeScript client 
        app.GenerateTypeScriptClient("/../../client/src/generated-client.ts").GetAwaiter().GetResult();
    }
}