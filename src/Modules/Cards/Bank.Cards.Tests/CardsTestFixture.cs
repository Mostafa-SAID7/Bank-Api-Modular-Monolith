namespace Bank.Cards.Tests;

using Bank.Cards.Infrastructure.Services;
using System;

/// <summary>
/// Test fixture for Cards module integration tests
/// Provides in-memory database and mediator setup
/// </summary>
public class CardsTestFixture : IAsyncLifetime
{
    private CardsDbContext? _dbContext;
    private IServiceProvider? _serviceProvider;
    public IMediator? Mediator { get; private set; }

    public async Task InitializeAsync()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<CardsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CardsDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        // Register services
        var services = new ServiceCollection();
        services.AddScoped(_ => _dbContext);
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardProductRepository, CardProductRepository>();
        services.AddScoped<ICardTransactionRepository, CardTransactionRepository>();
        services.AddScoped<ICardBlockRepository, CardBlockRepository>();
        services.AddScoped<ICardNumberGenerator, CardNumberGenerator>();
        services.AddScoped<ICardEncryptionService, CardEncryptionService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IssueCardCommand>());

        _serviceProvider = services.BuildServiceProvider();
        Mediator = _serviceProvider.GetRequiredService<IMediator>();
    }

    public async Task DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
        (_serviceProvider as IDisposable)?.Dispose();
    }

    /// <summary>
    /// Get the test database context
    /// </summary>
    public CardsDbContext GetDbContext()
    {
        return _dbContext ?? throw new InvalidOperationException("DbContext not initialized");
    }

    /// <summary>
    /// Seed sample card products for testing
    /// </summary>
    public async Task SeedCardProductsAsync()
    {
        if (_dbContext == null) return;

        var visaDebit = CardProduct.Create(
            "Visa Debit Card",
            "Standard Visa debit card",
            CardType.Debit,
            CardBrand.Visa,
            0m,
            0m,
            50000m,
            10000m,
            5,
            true,
            true);

        var mastercardCredit = CardProduct.Create(
            "MasterCard Credit",
            "Premium MasterCard credit card",
            CardType.Credit,
            CardBrand.MasterCard,
            50m,
            1.5m,
            100000m,
            50000m,
            5,
            true,
            true);

        _dbContext.CardProducts.Add(visaDebit);
        _dbContext.CardProducts.Add(mastercardCredit);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Create and save a valid card product for testing
    /// </summary>
    public CardProduct CreateAndSaveCardProduct(Guid productId, CardType cardType = CardType.Debit)
    {
        if (_dbContext == null)
            throw new InvalidOperationException("DbContext not initialized");

        var product = CardProduct.Create(
            $"Test {cardType} Card",
            $"Test {cardType} product",
            cardType,
            CardBrand.Visa,
            0m,
            0m,
            50000m,
            10000m,
            5,
            true,
            true);

        // Override Id for testing
        var property = typeof(CardProduct).GetProperty("Id");
        property?.SetValue(product, productId);

        _dbContext.CardProducts.Add(product);
        _dbContext.SaveChanges();
        return product;
    }
}
