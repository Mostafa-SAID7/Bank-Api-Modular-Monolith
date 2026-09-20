using Bank.Cards.Presentation.Validators;

namespace Bank.Cards.Presentation;

/// <summary>
/// Extension methods for registering presentation layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCardsPresentationServices(
        this IServiceCollection services)
    {
        // Register validators
        services.AddValidatorsFromAssemblyContaining<IssueCardRequestValidator>();

        return services;
    }
}
