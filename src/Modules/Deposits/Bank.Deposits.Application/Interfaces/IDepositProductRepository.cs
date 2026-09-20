namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Repository interface for DepositProduct persistence
/// </summary>
public interface IDepositProductRepository
{
    /// <summary>
    /// Add a new deposit product to persistence
    /// </summary>
    Task AddAsync(DepositProduct depositProduct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a deposit product by ID
    /// </summary>
    Task<DepositProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active deposit products
    /// </summary>
    Task<List<DepositProduct>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all deposit products
    /// </summary>
    Task<List<DepositProduct>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing deposit product
    /// </summary>
    Task UpdateAsync(DepositProduct depositProduct, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a deposit product
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
