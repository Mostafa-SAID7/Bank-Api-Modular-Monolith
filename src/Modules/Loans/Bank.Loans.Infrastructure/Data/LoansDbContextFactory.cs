namespace Bank.Loans.Infrastructure.Data;

/// <summary>
/// Factory for creating LoansDbContext instances
/// Used by EF Core tooling for migrations
/// </summary>
public class LoansDbContextFactory : IDesignTimeDbContextFactory<LoansDbContext>
{
    public LoansDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LoansDbContext>();
        
        // Use a default connection string for migrations
        // This will be overridden by appsettings at runtime
        optionsBuilder.UseNpgsql("Host=localhost;Database=bank_loans;Username=postgres;Password=postgres");

        return new LoansDbContext(optionsBuilder.Options);
    }
}
