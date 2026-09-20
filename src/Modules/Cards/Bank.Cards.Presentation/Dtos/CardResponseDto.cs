namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Response DTO for card details
/// </summary>
public record CardResponseDto(
    Guid Id,
    Guid CustomerId,
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
    bool InternationalEnabled,
    bool OnlineTransactionsEnabled,
    bool ContactlessEnabled);
