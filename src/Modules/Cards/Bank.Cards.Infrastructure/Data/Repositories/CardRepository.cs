namespace Bank.Cards.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for Card aggregate persistence operations
/// Implements ICardRepository interface from Application layer
/// </summary>
public sealed class CardRepository : ICardRepository
{
    private readonly CardsDbContext _context;

    public CardRepository(CardsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Card card, CancellationToken cancellationToken = default)
    {
        await _context.Cards.AddAsync(card, cancellationToken);
    }

    public async Task UpdateAsync(Card card, CancellationToken cancellationToken = default)
    {
        _context.Cards.Update(card);
        await Task.CompletedTask;
    }

    public async Task<Card?> GetByIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Card>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.LinkedAccountId == accountId)
            .ToListAsync(cancellationToken);
    }
}
