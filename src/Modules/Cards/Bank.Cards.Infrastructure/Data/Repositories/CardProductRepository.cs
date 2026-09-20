namespace Bank.Cards.Infrastructure.Data.Repositories;

/// <summary>
/// Repository for CardProduct entity persistence operations
/// Implements ICardProductRepository interface from Application layer
/// </summary>
public sealed class CardProductRepository : ICardProductRepository
{
    private readonly CardsDbContext _context;

    public CardProductRepository(CardsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CardProduct product, CancellationToken cancellationToken = default)
    {
        await _context.CardProducts.AddAsync(product, cancellationToken);
    }

    public async Task UpdateAsync(CardProduct product, CancellationToken cancellationToken = default)
    {
        _context.CardProducts.Update(product);
        await Task.CompletedTask;
    }

    public async Task<CardProduct?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.CardProducts
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<IEnumerable<CardProduct>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardProducts
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }
}
