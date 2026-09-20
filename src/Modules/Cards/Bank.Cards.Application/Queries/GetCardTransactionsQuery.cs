namespace Bank.Cards.Application.Queries;

/// <summary>
/// Query to retrieve transactions for a card within a date range
/// </summary>
public sealed record GetCardTransactionsQuery(
    Guid CardId,
    DateTime FromDate,
    DateTime ToDate) : IRequest<IEnumerable<CardTransactionDto>>;
