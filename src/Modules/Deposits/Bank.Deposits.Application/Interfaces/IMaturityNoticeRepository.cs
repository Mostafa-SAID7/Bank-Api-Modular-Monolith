namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Repository interface for MaturityNotice persistence
/// </summary>
public interface IMaturityNoticeRepository
{
    /// <summary>
    /// Add a new maturity notice to persistence
    /// </summary>
    Task AddAsync(MaturityNotice maturityNotice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a maturity notice by ID
    /// </summary>
    Task<MaturityNotice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a maturity notice by fixed deposit ID
    /// </summary>
    Task<MaturityNotice?> GetByFixedDepositIdAsync(Guid fixedDepositId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all pending maturity notices (not yet sent)
    /// </summary>
    Task<List<MaturityNotice>> GetPendingNoticesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get maturity notices for a customer
    /// </summary>
    Task<List<MaturityNotice>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing maturity notice
    /// </summary>
    Task UpdateAsync(MaturityNotice maturityNotice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a maturity notice
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
