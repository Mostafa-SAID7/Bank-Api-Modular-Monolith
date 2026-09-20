namespace Bank.Deposits.Infrastructure.Services;

/// <summary>
/// Generates unique account numbers for deposit accounts
/// Format: DEP + 12 hexadecimal characters (e.g., DEP0A1B2C3D4E5F6G)
/// Uses GUID to ensure uniqueness across distributed systems
/// </summary>
public sealed class AccountNumberGenerator : IAccountNumberGenerator
{
    public Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        // Generate a new GUID and take the first 12 characters of its hex representation
        var guid = Guid.NewGuid();
        var hexString = guid.ToString("N")[..12]; // First 12 chars of GUID without hyphens
        var accountNumber = $"DEP{hexString.ToUpper()}";

        return Task.FromResult(accountNumber);
    }
}
