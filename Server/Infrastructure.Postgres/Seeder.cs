using Application.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres;

public class Seeder(MyDbContext context, IOptionsMonitor<AppOptions> optionsMonitor) : ISeeder
{
    public async Task Seed()
    {
        await context.Database.EnsureCreatedAsync();
        var outputPath = Path.Combine(Directory.GetCurrentDirectory() +
                                      "/../Infrastructure.Postgres.Scaffolding/current_schema.sql");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        await File.WriteAllTextAsync(outputPath,
            "-- This schema is generated based on the current DBContext. Please check the class " + nameof(Seeder) +
            " to see.\n" +
            "" + context.Database.GenerateCreateScript());
    }
}