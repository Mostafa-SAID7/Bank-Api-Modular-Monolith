namespace Bank.Statements.Infrastructure;

using Bank.Statements.Application.Interfaces;
using Bank.Statements.Infrastructure.Data;
using Bank.Statements.Infrastructure.Data.Repositories;
using Bank.Statements.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddStatementsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<StatementsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "statements")));

        services.AddScoped<IStatementRepository, StatementRepository>();
        services.AddScoped<IStatementLineRepository, StatementLineRepository>();
        services.AddScoped<IStatementScheduleRepository, StatementScheduleRepository>();
        services.AddScoped<IStatementRecipientRepository, StatementRecipientRepository>();
        
        services.AddScoped<IStatementGenerator, StatementGenerator>();
        services.AddScoped<IStatementFormatter, StatementFormatter>();

        return services;
    }
}
