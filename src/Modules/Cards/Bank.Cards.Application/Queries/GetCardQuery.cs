namespace Bank.Cards.Application.Queries;

/// <summary>
/// Query to retrieve a card by ID
/// </summary>
public sealed record GetCardQuery(Guid CardId) : IRequest<CardDto>;
