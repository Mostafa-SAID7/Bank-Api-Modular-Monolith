using Bank.Identity.Application.Interfaces;
using Bank.Identity.Domain.Entities;
using Bank.Identity.Infrastructure.Data;
using Bank.Identity.Infrastructure.Data.Repositories;
using Bank.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Identity.Infrastructure;

/// <summary>
/// Extension methods for registering Identity infrastructure services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string jwtSecret,
        string jwtIssuer,
        string jwtAudience)
    {
        // Register DbContext
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "identity"))
        );

        // Register ASP.NET Core Identity
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        // Configure Identity options
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        // Register services
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped(provider =>
            new JwtTokenService(jwtSecret, jwtIssuer, jwtAudience)
        );

        return services;
    }
}
