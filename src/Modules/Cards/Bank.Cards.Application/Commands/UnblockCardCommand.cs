namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to unblock a previously blocked card
/// </summary>
public sealed record UnblockCardCommand(
    Guid CardId) : IRequest<Card>;
