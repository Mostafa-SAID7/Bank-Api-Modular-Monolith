using Bank.Cards.Infrastructure.Data;
using Bank.Cards.Infrastructure.Data.Repositories;
using Bank.Cards.Infrastructure.Services;

namespace Bank.Cards.Infrastructure;

/// <summary>
/// Registers Cards infrastructure services (DbContext, repositories, services)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCardsInfrastructure(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // Register DbContext
        services.AddDbContext<CardsDbContext>((serviceProvider, options) =>
        {
            // Use the provided connection string or fall back to reading from configuration
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                // Connection string will be provided at application startup
                options.UseNpgsql();
            }
        });

        // Register repositories
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardProductRepository, CardProductRepository>();
        services.AddScoped<ICardTransactionRepository, CardTransactionRepository>();
        services.AddScoped<ICardBlockRepository, CardBlockRepository>();

        // Register services
        services.AddScoped<ICardNumberGenerator, CardNumberGenerator>();
        services.AddScoped<ICardEncryptionService, CardEncryptionService>();

        return services;
    }
}
