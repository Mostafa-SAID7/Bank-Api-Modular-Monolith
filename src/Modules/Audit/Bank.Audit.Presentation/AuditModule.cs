namespace Bank.Audit.Presentation;

using Bank.Audit.Presentation.Endpoints;
using Bank.Audit.Application;
using Bank.Audit.Infrastructure;

/// <summary>
/// Audit module registration for the modular monolith
/// Registers all infrastructure, application, and presentation services
/// without exposing internal details to other modules
/// </summary>
public sealed class AuditModule : IModule
{
    public string Name => "Audit";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure (DbContext, repositories, services)
        services.AddAuditInfrastructure(configuration);

        // Register application layer (MediatR handlers)
        services.AddAuditApplicationServices();

        // Register presentation layer services (validators)
        services.AddAuditPresentationServices();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Map all Audit endpoints
        app.MapAuditEndpoints();
    }
}
