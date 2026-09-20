namespace Bank.Cards.Application.Interfaces;

/// <summary>
/// Repository interface for CardBlock entity
/// </summary>
public interface ICardBlockRepository
{
    Task AddAsync(CardBlock block, CancellationToken cancellationToken = default);
    Task<IEnumerable<CardBlock>> GetByCardIdAsync(Guid cardId, CancellationToken cancellationToken = default);
}
