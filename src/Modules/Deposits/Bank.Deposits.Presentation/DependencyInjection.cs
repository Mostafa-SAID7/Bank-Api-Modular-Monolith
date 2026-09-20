using Bank.Deposits.Presentation.Validators;

namespace Bank.Deposits.Presentation;

/// <summary>
/// Extension methods for registering presentation layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDepositsPresentationServices(
        this IServiceCollection services)
    {
        // Register validators
        services.AddValidatorsFromAssemblyContaining<CreateDepositRequestValidator>();

        return services;
    }
}
