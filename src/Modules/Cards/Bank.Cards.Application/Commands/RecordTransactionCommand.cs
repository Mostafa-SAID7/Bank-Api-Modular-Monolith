namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to record a transaction against card limits
/// </summary>
public sealed record RecordTransactionCommand(
    Guid CardId,
    decimal Amount,
    TransactionType Type,
    bool IsInternational = false) : IRequest<Unit>;
