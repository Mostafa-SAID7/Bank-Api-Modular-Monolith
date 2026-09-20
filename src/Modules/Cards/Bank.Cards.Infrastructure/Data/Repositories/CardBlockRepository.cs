namespace Bank.Cards.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for CardBlock entity persistence operations
/// Implements ICardBlockRepository interface from Application layer
/// </summary>
public sealed class CardBlockRepository : ICardBlockRepository
{
    private readonly CardsDbContext _context;

    public CardBlockRepository(CardsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CardBlock block, CancellationToken cancellationToken = default)
    {
        await _context.CardBlocks.AddAsync(block, cancellationToken);
    }

    public async Task<IEnumerable<CardBlock>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return await _context.CardBlocks
            .Where(b => b.CardId == cardId)
            .OrderByDescending(b => b.BlockedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
