using Bank.Api.Extensions.Configuration;
using Bank.Api.Extensions.DependencyInjection;
using Bank.Api.Extensions.Infrastructure;
using Bank.BuildingBlocks.Application.Modules;
using Bank.CoreBanking.Presentation;
using Bank.Identity.Presentation;
using Bank.Payments.Infrastructure;
using Bank.Loans.Presentation;
using Bank.Cards.Presentation;
using Bank.Deposits.Presentation;
using Bank.Statements.Presentation;
using Bank.Audit.Presentation;
using Bank.Notifications.Infrastructure;
using Bank.Notifications.Presentation;

namespace Bank.Api.Extensions;

/// <summary>
/// Main orchestrator for service registrations - delegates to specialized extension classes
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all Bank API services in the correct order
    /// </summary>
    public static IServiceCollection AddBankApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core infrastructure
        services.AddDatabaseServices(configuration);
        services.AddCachingServices(configuration);
        services.AddAuthenticationServices(configuration);
        
        // Data access
        services.AddRepositoryServices();
        
        // Business logic
        services.AddApplicationServices(configuration);
        services.AddInfrastructureServices();
        
        // CQRS and validation
        services.AddCqrsServices();
        services.AddAutoMapperServices();
        
        // Background jobs
        services.AddBackgroundJobServices(configuration);
        
        // API features
        services.AddApiDocumentationServices();
        services.AddCorsServices(configuration);

        // Module composition: Register all modules to enable service injection.
        // Each module owns its infrastructure, application, and presentation layers.
        services.AddModules(
            configuration,
            new CoreBankingModule(),
            new IdentityModule(),
            new PaymentsModule(),
            new LoansModule(),
            new CardsModule(),
            new DepositsModule(),
            new StatementsModule(),
            new AuditModule(),
            new NotificationsModule());

        return services;
    }
}