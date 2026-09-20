using Bank.Statements.Application;
using Bank.Statements.Infrastructure;
using Bank.Statements.Presentation.Requests;
using Bank.Statements.Presentation.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Bank.Statements.Presentation;

/// <summary>
/// Statements module extension
/// Registers all Statements module services and endpoints
/// </summary>
public static class StatementsModule
{
    /// <summary>
    /// Add Statements module services to dependency injection container
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
    /// Map Statements module endpoints
    /// </summary>
    public static void MapStatementsModule(this WebApplication app)
    {
        Endpoints.StatementsEndpoints.MapStatementsEndpoints(app);
    }
}

