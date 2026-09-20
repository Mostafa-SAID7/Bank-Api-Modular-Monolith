namespace Bank.Cards.Application.Queries;

/// <summary>
/// Query to validate if a card can process a specific transaction
/// </summary>
public sealed record ValidateCardForTransactionQuery(
    Guid CardId,
    decimal Amount,
    TransactionType Type,
    bool IsInternational = false,
    bool IsContactless = false) : IRequest<bool>;
