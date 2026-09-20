using Bank.CoreBanking.Domain.Entities;
using Bank.CoreBanking.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.CoreBanking.Tests;

/// <summary>
/// Test fixture for CoreBanking module integration tests
/// Sets up WebApplicationFactory with in-memory database
/// </summary>
public class CoreBankingTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private CoreBankingDbContext? _dbContext;

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<CoreBankingDbContext>();
        
        // Ensure database is created
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
            // Remove the app's CoreBankingDbContext registration
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CoreBankingDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<CoreBankingDbContext>(
                options => options.UseInMemoryDatabase("CoreBankingTestDb"));
        });

        base.ConfigureWebHost(builder);
    }

    /// <summary>
    /// Helper method to add an account to the test database
    /// </summary>
    public async Task AddAccountAsync(Account account)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CoreBankingDbContext>();
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Helper method to add a transaction to the test database
    /// </summary>
    public async Task AddTransactionAsync(Transaction transaction)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CoreBankingDbContext>();
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Helper method to add a ledger entry to the test database
    /// </summary>
    public async Task AddLedgerEntryAsync(Ledger ledger)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CoreBankingDbContext>();
        dbContext.LedgerEntries.Add(ledger);
        await dbContext.SaveChangesAsync();
    }
}
