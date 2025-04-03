namespace Infrastructure.Postgres;

public interface ISeeder
{
    Task Seed();
}