using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Bank.Deposits.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext instances during migrations
/// Used by Entity Framework Core CLI commands (dotnet ef migrations)
/// </summary>
public sealed class DepositsDbContextFactory : IDesignTimeDbContextFactory<DepositsDbContext>
{
    public DepositsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DepositsDbContext>();

        // Use default PostgreSQL connection string for migrations
        // Override via environment variable or local settings
        var connectionString = Environment.GetEnvironmentVariable("DEPOSITS_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=bank_deposits;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new DepositsDbContext(optionsBuilder.Options);
    }
}
