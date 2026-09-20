namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Repository interface for InterestRate persistence
/// </summary>
public interface IInterestRateRepository
{
    /// <summary>
    /// Get current interest rate for a deposit type
    /// </summary>
    Task<InterestRate?> GetCurrentRateAsync(Guid depositTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get interest rate applicable for a specific balance
    /// </summary>
    Task<InterestRate?> GetRateForBalanceAsync(Guid depositTypeId, decimal balance, CancellationToken cancellationToken = default);
}
