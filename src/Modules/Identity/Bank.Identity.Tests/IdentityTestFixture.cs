using Bank.Identity.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Identity.Tests;

/// <summary>
/// Test fixture for Identity module integration tests
/// Sets up WebApplicationFactory with in-memory database
/// </summary>
public class IdentityTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IdentityDbContext? _dbContext;

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        
        // Ensure database is created and migrated
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.DisposeAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's IdentityDbContext registration
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<IdentityDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<IdentityDbContext>(
                options => options.UseInMemoryDatabase("IdentityTestDb"));
        });

        base.ConfigureWebHost(builder);
    }

    /// <summary>
    /// Helper method to add a user to the test database
    /// </summary>
    public async Task AddUserAsync(User user)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Helper method to add a role to the test database
    /// </summary>
    public async Task AddRoleAsync(Role role)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Helper method to add a session to the test database
    /// </summary>
    public async Task AddSessionAsync(Session session)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Helper method to get a user from the test database
    /// </summary>
    public async Task<User?> GetUserAsync(Guid userId)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        return await dbContext.Users.FindAsync(userId);
    }

    /// <summary>
    /// Helper method to get a session from the test database
    /// </summary>
    public async Task<Session?> GetSessionAsync(Guid sessionId)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        return await dbContext.Sessions.FindAsync(sessionId);
    }

    /// <summary>
    /// Helper method to clear all users from the test database
    /// </summary>
    public async Task ClearUsersAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Users.RemoveRange(dbContext.Users);
        await dbContext.SaveChangesAsync();
    }
}
