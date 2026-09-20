using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bank.Identity.Infrastructure.Data;

/// <summary>
/// Factory for creating IdentityDbContext instances (design-time for migrations)
/// </summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql("Server=localhost;Database=bank_identity;User Id=postgres;Password=postgres");
        return new IdentityDbContext(optionsBuilder.Options);
    }
}
