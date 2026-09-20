namespace Bank.Audit.Application;

using Bank.Audit.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditApplicationServices(this IServiceCollection services)
    {
        // Register MediatR handlers from assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateAuditLogCommand>());

        return services;
    }
}
