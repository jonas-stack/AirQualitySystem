using System.ComponentModel.DataAnnotations;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Application;

public static class AppOptionsExtensions
{
    public static AppOptions AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var appOptions = new AppOptions();
        configuration.GetSection("AppOptions").Bind(appOptions);

        services.Configure<AppOptions>(configuration.GetSection("AppOptions"));

        ICollection<ValidationResult> results = new List<ValidationResult>();
        var validated = Validator.TryValidateObject(appOptions, new ValidationContext(appOptions), results, true);
        if (!validated)
            throw new Exception(
                $"hey buddy, alex here. You're probably missing an environment variable / appsettings.json stuff / repo secret on github. Here's the technical error: " +
                $"{string.Join(", ", results.Select(r => r.ErrorMessage))}");

        return appOptions;
    }
}