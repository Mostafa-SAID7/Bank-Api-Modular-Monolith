namespace Bank.Cards.Application.Queries;

/// <summary>
/// Query to retrieve the block/unblock history of a card
/// </summary>
public sealed record GetCardBlockHistoryQuery(Guid CardId) : IRequest<IEnumerable<CardBlockDto>>;
