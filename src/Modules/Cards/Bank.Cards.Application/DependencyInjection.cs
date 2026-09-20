using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Cards.Application;

/// <summary>
/// Registers Cards application services (MediatR handlers, validators, pipelines)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCardsApplicationServices(this IServiceCollection services)
    {
        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CardsApplicationMarker>();
        });

        return services;
    }
}

/// <summary>
/// Marker class for assembly scanning
/// </summary>
public sealed class CardsApplicationMarker;
