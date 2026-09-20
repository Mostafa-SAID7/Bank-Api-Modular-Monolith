using Bank.Loans.Application;
using Bank.Loans.Presentation.Endpoints;
using Bank.Loans.Presentation.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Bank.Loans.Presentation;

/// <summary>
/// Loans module extension
/// Registers all Loans module services and endpoints
/// </summary>
public static class LoansModule
{
    /// <summary>
    /// Add Loans module services to dependency injection container
    /// </summary>
    public static IServiceCollection AddLoansModule(
        this IServiceCollection services,
        string? connectionString = null)
    {
        // Register infrastructure
        services.AddLoansInfrastructure(connectionString);

        // Register application layer MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplyForLoanCommand>());

        // Register FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateLoanRequestValidator>();

        return services;
    }

    /// <summary>
    /// Map Loans module endpoints
    /// </summary>
    public static void MapLoansModule(this WebApplication app)
    {
        app.MapLoansEndpoints();
    }
}
