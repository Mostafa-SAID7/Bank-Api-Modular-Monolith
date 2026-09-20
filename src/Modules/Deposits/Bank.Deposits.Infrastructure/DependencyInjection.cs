using Bank.Deposits.Infrastructure.Data;
using Bank.Deposits.Infrastructure.Data.Repositories;
using Bank.Deposits.Infrastructure.Services;

namespace Bank.Deposits.Infrastructure;

/// <summary>
/// Registers Deposits infrastructure services (DbContext, repositories, services)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDepositsInfrastructure(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // Register DbContext
        services.AddDbContext<DepositsDbContext>((serviceProvider, options) =>
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
        services.AddScoped<IDepositRepository, DepositRepository>();
        services.AddScoped<IDepositTypeRepository, DepositTypeRepository>();
        services.AddScoped<IInterestRateRepository, InterestRateRepository>();
        services.AddScoped<IFixedDepositRepository, FixedDepositRepository>();
        services.AddScoped<IDepositProductRepository, DepositProductRepository>();
        services.AddScoped<IMaturityNoticeRepository, MaturityNoticeRepository>();

        // Register services
        services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
        services.AddScoped<IInterestCalculationService, InterestCalculationService>();
        services.AddScoped<IFixedDepositNumberGenerator, FixedDepositNumberGenerator>();
        services.AddScoped<IMaturityProcessingService, MaturityProcessingService>();

        return services;
    }
}
