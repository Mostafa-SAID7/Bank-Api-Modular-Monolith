namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Service interface for maturity processing operations
/// </summary>
public interface IMaturityProcessingService
{
    /// <summary>
    /// Calculate maturity amount for a fixed deposit
    /// </summary>
    Task<decimal> CalculateMaturityAmountAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate accrued interest for a fixed deposit
    /// </summary>
    Task<decimal> CalculateAccruedInterestAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process maturity for all eligible fixed deposits
    /// </summary>
    Task ProcessMaturityForEligibleDepositsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Send maturity notification to customer
    /// </summary>
    Task SendMaturityNotificationAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);
}
