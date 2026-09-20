namespace Bank.Deposits.Application.Interfaces;

/// <summary>
/// Repository interface for Deposit aggregate persistence
/// </summary>
public interface IDepositRepository
{
    /// <summary>
    /// Add a new deposit to persistence
    /// </summary>
    Task AddAsync(Deposit deposit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a deposit by ID
    /// </summary>
    Task<Deposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a deposit by account number
    /// </summary>
    Task<Deposit?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all deposits for a customer
    /// </summary>
    Task<List<Deposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing deposit
    /// </summary>
    Task UpdateAsync(Deposit deposit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a deposit
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
