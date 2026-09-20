namespace Bank.Statements.Tests.Fixtures;

public sealed class StatementsTestFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;
    public StatementsDbContext DbContext { get; private set; } = null!;
    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("statements_test")
            .WithUsername("test")
            .WithPassword("test123")
            .Build();

        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<StatementsDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new StatementsDbContext(options);
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.DisposeAsync();
        }

        if (_container != null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    public Statement CreateTestStatement(
        Guid customerId,
        Guid accountId,
        DateTime? statementDate = null,
        StatementStatus status = StatementStatus.Pending)
    {
        return Statement.Create(
            customerId,
            accountId,
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow,
            1000m,
            StatementPeriod.Monthly,
            StatementFormat.PDF);
    }

    public StatementRecipient CreateTestRecipient(
        Guid statementId,
        string email = "test@example.com",
        DeliveryMethod method = DeliveryMethod.Email)
    {
        return StatementRecipient.Create(statementId, email, method);
    }

    public StatementSchedule CreateTestSchedule(
        Guid customerId,
        Guid accountId,
        StatementPeriod period = StatementPeriod.Monthly,
        StatementFormat format = StatementFormat.PDF,
        DeliveryMethod method = DeliveryMethod.Email)
    {
        return StatementSchedule.Create(
            customerId,
            accountId,
            period,
            format,
            method);
    }

    public async Task<T> AddAsync<T>(T entity) where T : class
    {
        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public async Task ClearAsync()
    {
        var connection = DbContext.Database.GetDbConnection();
        try
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    TRUNCATE statements.statement_recipients CASCADE;
                    TRUNCATE statements.statement_lines CASCADE;
                    TRUNCATE statements.statement_schedules CASCADE;
                    TRUNCATE statements.statements CASCADE;
                ";
                await command.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
