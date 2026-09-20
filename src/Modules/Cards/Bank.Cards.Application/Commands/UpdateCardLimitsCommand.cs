namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to update card transaction limits
/// </summary>
public sealed record UpdateCardLimitsCommand(
    Guid CardId,
    decimal DailyWithdrawalLimit,
    decimal DailyTransactionLimit,
    decimal DailyForeignTransactionLimit) : IRequest<Card>;
