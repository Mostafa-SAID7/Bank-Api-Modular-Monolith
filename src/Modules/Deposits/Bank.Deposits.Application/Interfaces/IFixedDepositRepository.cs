namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Repository interface for FixedDeposit aggregate persistence
/// </summary>
public interface IFixedDepositRepository
{
    /// <summary>
    /// Add a new fixed deposit to persistence
    /// </summary>
    Task AddAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a fixed deposit by ID
    /// </summary>
    Task<FixedDeposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a fixed deposit by fixed deposit number
    /// </summary>
    Task<FixedDeposit?> GetByFixedDepositNumberAsync(string fixedDepositNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all fixed deposits for a customer
    /// </summary>
    Task<List<FixedDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get fixed deposits that are ready for maturity processing
    /// </summary>
    Task<List<FixedDeposit>> GetFixedDepositsReadyForMaturityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing fixed deposit
    /// </summary>
    Task UpdateAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a fixed deposit
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
