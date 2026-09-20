namespace Bank.Audit.Infrastructure.Data;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<AuditEntry> AuditEntries { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }
    public DbSet<AuditConfiguration> AuditConfigurations { get; set; }
    public DbSet<UserAuditContext> UserAuditContexts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("audit");

        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new AuditEntryConfiguration());
        modelBuilder.ApplyConfiguration(new AuditTrailConfiguration());
        modelBuilder.ApplyConfiguration(new AuditConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new UserAuditContextConfiguration());
    }
}
