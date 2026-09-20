using Bank.BuildingBlocks.Application.Modules;
using Bank.Identity.Application.Handlers;
using Bank.Identity.Infrastructure;
using Bank.Identity.Infrastructure.Data;
using Bank.Identity.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Identity.Presentation;

/// <summary>
/// Identity module registration for the modular monolith
/// Registers all infrastructure, application, and presentation services
/// Handles user authentication, roles, and session management
/// </summary>
public sealed class IdentityModule : IModule
{
    public string Name => "Identity";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Get JWT configuration from appsettings
        var jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured");
        var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured");

        // Register infrastructure (DbContext, repositories, services)
        services.AddIdentityInfrastructure(
            configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not configured"),
            jwtSecret,
            jwtIssuer,
            jwtAudience
        );

        // Register command handlers
        services.AddScoped<RegisterUserCommandHandler>();
        services.AddScoped<LoginUserCommandHandler>();
        services.AddScoped<LogoutUserCommandHandler>();

        // Register query handlers
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<GetUserSessionsQueryHandler>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Map all Identity endpoints
        app.MapIdentityEndpoints();
    }
}
