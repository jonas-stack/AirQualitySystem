using Application.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Postgres;

public class Seeder(MyDbContext context) : ISeeder
{
    public async Task Seed()
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            var outputPath = Path.Combine(Directory.GetCurrentDirectory() +
                                          "/../Infrastructure.Postgres.Scaffolding/current_schema.sql");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await File.WriteAllTextAsync(outputPath,
                "-- This schema is generated based on the current DBContext. Please check the class " +
                nameof(Seeder) +
                " to see.\n" +
                "" + context.Database.GenerateCreateScript());
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("Could not connect to PostgreSQL.");
            Console.WriteLine($"Reason: {ex.Message}");

            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred while seeding the database.");
            Console.WriteLine($"Details: {ex.Message}");
            throw;
        }
    }
}