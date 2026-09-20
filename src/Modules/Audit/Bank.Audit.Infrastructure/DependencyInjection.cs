namespace Bank.Audit.Infrastructure;

using Bank.Audit.Domain.Repositories;
using Bank.Audit.Infrastructure.Data;
using Bank.Audit.Infrastructure.Repositories;
using Bank.Audit.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseNpgsql(connectionString, x =>
            {
                x.MigrationsHistoryTable("__EFMigrationsHistory", "audit");
                x.MigrationsAssembly(typeof(AuditDbContext).Assembly.GetName().Name);
            }));

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditEntryRepository, AuditEntryRepository>();
        services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
        services.AddScoped<IAuditConfigurationRepository, AuditConfigurationRepository>();

        services.AddScoped<IAuditPurgeService, AuditPurgeService>();
        services.AddScoped<IAuditExportService, AuditExportService>();
        services.AddScoped<IAuditQueryService, AuditQueryService>();

        return services;
    }
}
