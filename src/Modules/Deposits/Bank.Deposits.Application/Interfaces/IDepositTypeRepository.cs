namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Repository interface for DepositType persistence
/// </summary>
public interface IDepositTypeRepository
{
    /// <summary>
    /// Retrieve a deposit type by ID
    /// </summary>
    Task<DepositType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active deposit types
    /// </summary>
    Task<List<DepositType>> GetActiveTypesAsync(CancellationToken cancellationToken = default);
}
