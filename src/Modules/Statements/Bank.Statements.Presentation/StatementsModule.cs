using Bank.BuildingBlocks.Application.Modules;
using Bank.Statements.Application;
using Bank.Statements.Infrastructure;
using Bank.Statements.Presentation.Endpoints;
using Bank.Statements.Presentation.Requests;
using Bank.Statements.Presentation.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Statements.Presentation;

/// <summary>
/// Statements module registration for the modular monolith
/// Registers all infrastructure, application, and presentation services
/// without exposing internal details to other modules
/// </summary>
public sealed class StatementsModule : IModule
{
    public string Name => "Statements";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure (DbContext, repositories, services)
        services.AddStatementsInfrastructure(
            configuration.GetConnectionString("DefaultConnection")
        );

        // Register application layer services (MediatR handlers, pipelines)
        services.AddStatementsApplicationServices();

        // Register presentation layer services (validators)
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<GenerateStatementValidator>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Map all Statements endpoints
        app.MapStatementsEndpoints();
    }
}

/// <summary>
/// Extension methods for Statements module (legacy static extensions)
/// Keep for backward compatibility
/// </summary>
public static class StatementsModuleExtensions
{
    /// <summary>
    /// Add Statements module services to dependency injection container (legacy)
    /// </summary>
    public static IServiceCollection AddStatementsModule(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // Register infrastructure
        services.AddStatementsInfrastructure(connectionString ?? string.Empty);

        // Register application layer MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GenerateStatementCommand>());

        // Register FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<GenerateStatementValidator>();

        return services;
    }

    /// <summary>
    /// Map Statements module endpoints (legacy)
    /// </summary>
    public static void MapStatementsModule(this WebApplication app)
    {
        StatementsEndpoints.MapStatementsEndpoints(app);
    }
}

