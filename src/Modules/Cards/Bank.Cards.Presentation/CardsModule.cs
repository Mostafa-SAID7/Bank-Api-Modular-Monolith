using Bank.Cards.Presentation.Endpoints;

namespace Bank.Cards.Presentation;

/// <summary>
/// Cards module registration for the modular monolith
/// Registers all infrastructure, application, and presentation services
/// without exposing internal details to other modules
/// </summary>
public sealed class CardsModule : IModule
{
    public string Name => "Cards";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure (DbContext, repositories, services)
        services.AddCardsInfrastructure(
            configuration.GetConnectionString("DefaultConnection")
        );

        // Register presentation layer services (validators)
        services.AddCardsPresentationServices();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Map all Cards endpoints
        app.MapCardsEndpoints();
    }
}
