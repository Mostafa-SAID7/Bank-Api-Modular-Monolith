using Bank.Deposits.Presentation.Endpoints;

namespace Bank.Deposits.Presentation;

/// <summary>
/// Deposits module registration for the modular monolith
/// Registers all infrastructure, application, and presentation services
/// without exposing internal details to other modules
/// </summary>
public sealed class DepositsModule : IModule
{
    public string Name => "Deposits";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure (DbContext, repositories, services)
        services.AddDepositsInfrastructure(
            configuration.GetConnectionString("DefaultConnection")
        );

        // Register presentation layer services (validators)
        services.AddDepositsPresentationServices();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Map all Deposits endpoints
        app.MapDepositsEndpoints();
    }
}
