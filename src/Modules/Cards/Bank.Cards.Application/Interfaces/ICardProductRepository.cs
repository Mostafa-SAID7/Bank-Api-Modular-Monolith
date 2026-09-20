namespace Bank.Cards.Application.Interfaces;

/// <summary>
/// Repository interface for CardProduct entity
/// </summary>
public interface ICardProductRepository
{
    Task AddAsync(CardProduct product, CancellationToken cancellationToken = default);
    Task UpdateAsync(CardProduct product, CancellationToken cancellationToken = default);
    Task<CardProduct?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CardProduct>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
