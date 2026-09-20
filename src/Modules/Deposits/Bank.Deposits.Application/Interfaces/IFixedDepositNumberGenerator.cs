namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Service interface for generating fixed deposit numbers
/// </summary>
public interface IFixedDepositNumberGenerator
{
    /// <summary>
    /// Generate a unique fixed deposit number
    /// </summary>
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
