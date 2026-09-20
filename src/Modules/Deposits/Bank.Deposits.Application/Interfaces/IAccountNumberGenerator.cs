namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Service interface for generating unique deposit account numbers
/// </summary>
public interface IAccountNumberGenerator
{
    /// <summary>
    /// Generate a unique deposit account number
    /// Format: DEP + 12 hexadecimal characters (e.g., DEP123ABC456DEF)
    /// </summary>
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
