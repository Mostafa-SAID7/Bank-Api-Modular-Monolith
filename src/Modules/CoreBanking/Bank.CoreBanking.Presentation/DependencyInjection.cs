using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Bank.CoreBanking.Presentation.Validators;

namespace Bank.CoreBanking.Presentation;

/// <summary>
/// Extension methods for registering presentation layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCoreBankingPresentationServices(
        this IServiceCollection services)
    {
        // Register validators
        services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();

        return services;
    }
}
