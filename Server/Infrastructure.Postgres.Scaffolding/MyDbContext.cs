using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }
}