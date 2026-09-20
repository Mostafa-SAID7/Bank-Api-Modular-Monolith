using Bank.BuildingBlocks.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.CoreBanking.Application;

/// <summary>
/// Registers Core Banking application services (MediatR handlers, validators, pipelines)
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCoreBankingApplicationServices(this IServiceCollection services)
    {
        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CoreBankingApplicationMarker>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        return services;
    }
}

/// <summary>
/// Marker class for assembly scanning
/// </summary>
public sealed class CoreBankingApplicationMarker;
