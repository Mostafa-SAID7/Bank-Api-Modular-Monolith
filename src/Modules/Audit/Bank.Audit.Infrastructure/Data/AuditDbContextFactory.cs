namespace Bank.Audit.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();

        // Default connection string for migrations
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=audit;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString, x =>
        {
            x.MigrationsHistoryTable("__ef_migrations_history", "audit");
            x.MigrationsAssembly(typeof(AuditDbContext).Assembly.GetName().Name);
        });

        return new AuditDbContext(optionsBuilder.Options);
    }
}
