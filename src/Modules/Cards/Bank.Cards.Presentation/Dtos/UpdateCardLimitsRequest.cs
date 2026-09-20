namespace Bank.Cards.Presentation.Dtos;

/// <summary>
/// Request DTO for updating card limits
/// </summary>
public record UpdateCardLimitsRequest(
    Guid CardId,
    decimal DailyWithdrawalLimit,
    decimal DailyTransactionLimit,
    decimal DailyForeignTransactionLimit);
