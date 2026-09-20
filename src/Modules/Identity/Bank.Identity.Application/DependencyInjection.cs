using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Identity.Application;

/// <summary>
/// Registers Identity application services (MediatR handlers, validators, pipelines)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplicationServices(this IServiceCollection services)
    {
        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<IdentityApplicationMarker>();
        });

        return services;
    }
}

/// <summary>
/// Marker class for assembly scanning
/// </summary>
public sealed class IdentityApplicationMarker;
