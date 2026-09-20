namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to block a card
/// </summary>
public sealed record BlockCardCommand(
    Guid CardId,
    BlockReason Reason,
    string Description = "") : IRequest<Card>;
