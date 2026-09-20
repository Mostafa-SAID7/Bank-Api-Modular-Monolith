using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Statements.Application;

/// <summary>
/// Registers Statements application services (MediatR handlers, validators, pipelines)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddStatementsApplicationServices(this IServiceCollection services)
    {
        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<StatementsApplicationMarker>();
        });

        return services;
    }
}

/// <summary>
/// Marker class for assembly scanning
/// </summary>
public sealed class StatementsApplicationMarker;
