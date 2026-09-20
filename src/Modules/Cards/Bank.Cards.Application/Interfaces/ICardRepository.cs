namespace Bank.Cards.Application.Interfaces;

/// <summary>
/// Repository interface for Card aggregate
/// </summary>
public interface ICardRepository
{
    Task AddAsync(Card card, CancellationToken cancellationToken = default);
    Task UpdateAsync(Card card, CancellationToken cancellationToken = default);
    Task<Card?> GetByIdAsync(Guid cardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Card>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
}
