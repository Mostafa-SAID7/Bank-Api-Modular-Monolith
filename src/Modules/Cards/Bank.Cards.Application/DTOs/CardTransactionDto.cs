namespace Bank.Cards.Application.DTOs;

/// <summary>
/// DTO for CardTransaction entity
/// </summary>
public sealed record CardTransactionDto(
    Guid Id,
    Guid CardId,
    decimal Amount,
    TransactionType Type,
    TransactionStatus Status,
    bool IsInternational,
    bool IsContactless,
    string? Merchant,
    DateTime TransactionDateUtc,
    DateTime? ReversalDateUtc);
