namespace Bank.Deposits.Tests;

/// <summary>
/// Test fixture for Deposits module integration tests
/// Provides in-memory database and mediator setup
/// </summary>
public class DepositsTestFixture : IAsyncLifetime
{
    private DepositsDbContext? _dbContext;
    private IServiceProvider? _serviceProvider;
    public IMediator? Mediator { get; private set; }

    public async Task InitializeAsync()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<DepositsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DepositsDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        // Register services
        var services = new ServiceCollection();
        services.AddScoped(_ => _dbContext);
        services.AddScoped<IDepositRepository, DepositRepository>();
        services.AddScoped<IDepositTypeRepository, DepositTypeRepository>();
        services.AddScoped<IInterestRateRepository, InterestRateRepository>();
        services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
        services.AddScoped<IInterestCalculationService, InterestCalculationService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<OpenDepositCommand>());

        _serviceProvider = services.BuildServiceProvider();
        Mediator = _serviceProvider.GetRequiredService<IMediator>();
    }

    public async Task DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
        _serviceProvider?.Dispose();
    }

    /// <summary>
    /// Seed sample deposit types for testing
    /// </summary>
    public async Task SeedDepositTypesAsync()
    {
        if (_dbContext == null) return;

        var savingsType = new DepositType
        {
            Id = Guid.NewGuid(),
            Name = "Savings Account",
            Description = "Regular savings account",
            MinimumBalance = 100m,
            MaximumBalance = null,
            IsFixedDeposit = false,
            DefaultTermMonths = null,
            AllowsEarlyWithdrawal = true,
            EarlyWithdrawalPenaltyPercent = null,
            AllowsPrematureClosure = true,
            PrematureClosurePenaltyPercent = null,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var fixedDepositType = new DepositType
        {
            Id = Guid.NewGuid(),
            Name = "Fixed Deposit - 12 Months",
            Description = "Fixed deposit for 12 months",
            MinimumBalance = 1000m,
            MaximumBalance = 1000000m,
            IsFixedDeposit = true,
            DefaultTermMonths = 12,
            AllowsEarlyWithdrawal = false,
            EarlyWithdrawalPenaltyPercent = 2m,
            AllowsPrematureClosure = true,
            PrematureClosurePenaltyPercent = 1.5m,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _dbContext.DepositTypes.AddRangeAsync(savingsType, fixedDepositType);
        await _dbContext.SaveChangesAsync();
    }

    public DepositsDbContext GetDbContext() => _dbContext ?? throw new InvalidOperationException("DbContext not initialized");
}
