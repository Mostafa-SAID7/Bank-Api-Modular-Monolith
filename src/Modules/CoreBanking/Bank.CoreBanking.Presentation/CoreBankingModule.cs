using Bank.BuildingBlocks.Application.Modules;
using Bank.CoreBanking.Application;
using Bank.CoreBanking.Infrastructure;
using Bank.CoreBanking.Infrastructure.Data;
using Bank.CoreBanking.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.CoreBanking.Presentation;

/// <summary>
/// Core Banking module registration for the modular monolith
/// Registers all infrastructure, application, and presentation services
/// without exposing internal details to other modules
/// </summary>
public sealed class CoreBankingModule : IModule
{
    public string Name => "CoreBanking";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure (DbContext, repositories, publishers)
        services.AddCoreBankingInfrastructure(
            configuration.GetConnectionString("DefaultConnection")
        );

        // Register application layer services (MediatR handlers, pipelines)
        services.AddCoreBankingApplicationServices();

        // Register presentation layer services (validators)
        services.AddCoreBankingPresentationServices();

        // DbContext is already registered in infrastructure, just ensure it's available
        // Apply migrations if needed (deferred to startup or manual migration)
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Map all CoreBanking endpoints
        app.MapCoreBankingEndpoints();
    }
}
