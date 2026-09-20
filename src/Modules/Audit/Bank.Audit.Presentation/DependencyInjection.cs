namespace Bank.Audit.Presentation;

using Bank.Audit.Presentation.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditPresentationServices(this IServiceCollection services)
    {
        // Register validators from assembly
        services.AddValidatorsFromAssemblyContaining<CreateAuditLogRequestValidator>();

        return services;
    }
}
