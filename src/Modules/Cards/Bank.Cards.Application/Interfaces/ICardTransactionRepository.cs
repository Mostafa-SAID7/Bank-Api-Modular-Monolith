namespace Bank.Cards.Application.Interfaces;

/// <summary>
/// Repository interface for CardTransaction entity
/// </summary>
public interface ICardTransactionRepository
{
    Task AddAsync(CardTransaction transaction, CancellationToken cancellationToken = default);
    Task<CardTransaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CardTransaction>> GetByCardIdAsync(Guid cardId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
