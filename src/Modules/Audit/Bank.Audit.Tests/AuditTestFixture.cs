namespace Bank.Audit.Tests;

using Bank.Audit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

public sealed class AuditTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private AuditDbContext? _dbContext;

    public AuditTestFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("audit_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public AuditDbContext DbContext
    {
        get => _dbContext ?? throw new InvalidOperationException("DbContext not initialized");
    }

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AuditDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        _dbContext = new AuditDbContext(options);
        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }

        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}
