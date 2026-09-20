using Bank.Loans.Infrastructure.Data;
using Bank.Loans.Infrastructure.Data.Repositories;
using Bank.Loans.Infrastructure.Services;

namespace Bank.Loans.Infrastructure;

/// <summary>
/// Registers Loans infrastructure services (DbContext, repositories, services)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddLoansInfrastructure(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // Register DbContext
        services.AddDbContext<LoansDbContext>((serviceProvider, options) =>
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
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ILoanProductRepository, LoanProductRepository>();

        // Register services
        services.AddScoped<IEMICalculationService, EMICalculationService>();
        services.AddScoped<ILoanNumberGenerator, LoanNumberGenerator>();

        return services;
    }
}
