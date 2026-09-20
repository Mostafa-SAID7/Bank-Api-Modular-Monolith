namespace Bank.Cards.Application.DTOs;

/// <summary>
/// DTO for Card entity
/// </summary>
public sealed record CardDto(
    Guid Id,
    Guid CustomerId,
    Guid LinkedAccountId,
    Guid CardProductId,
    string MaskedPan,
    CardType CardType,
    CardBrand CardBrand,
    CardStatus Status,
    string HolderName,
    DateTime IssuedDateUtc,
    DateTime ExpiryDateUtc,
    DateTime? ActivatedDateUtc,
    DateTime? BlockedDateUtc,
    DateTime? ClosedDateUtc,
    decimal DailyWithdrawalLimit,
    decimal DailyTransactionLimit,
    decimal DailyForeignTransactionLimit,
    bool InternationalEnabled,
    bool OnlineTransactionsEnabled,
    bool ContactlessEnabled,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
