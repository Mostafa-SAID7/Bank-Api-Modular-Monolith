namespace Bank.Cards.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for CardTransaction entity persistence operations
/// Implements ICardTransactionRepository interface from Application layer
/// </summary>
public sealed class CardTransactionRepository : ICardTransactionRepository
{
    private readonly CardsDbContext _context;

    public CardTransactionRepository(CardsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CardTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.CardTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task<CardTransaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.CardTransactions
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
    }

    public async Task<IEnumerable<CardTransaction>> GetByCardIdAsync(
        Guid cardId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.CardTransactions
            .Where(t => t.CardId == cardId &&
                        t.TransactionDateUtc >= fromDate &&
                        t.TransactionDateUtc <= toDate)
            .OrderByDescending(t => t.TransactionDateUtc)
            .ToListAsync(cancellationToken);
    }
}
