namespace Bank.Statements.Infrastructure.Data;

using Bank.Statements.Domain.Entities;
using Bank.Statements.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

public sealed class StatementsDbContext : DbContext
{
    public StatementsDbContext(DbContextOptions<StatementsDbContext> options) : base(options)
    {
    }

    public DbSet<Statement> Statements { get; set; } = null!;
    public DbSet<StatementLine> StatementLines { get; set; } = null!;
    public DbSet<StatementSchedule> StatementSchedules { get; set; } = null!;
    public DbSet<StatementRecipient> StatementRecipients { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("statements");

        modelBuilder.ApplyConfiguration(new StatementConfiguration());
        modelBuilder.ApplyConfiguration(new StatementLineConfiguration());
        modelBuilder.ApplyConfiguration(new StatementScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new StatementRecipientConfiguration());
    }
}
