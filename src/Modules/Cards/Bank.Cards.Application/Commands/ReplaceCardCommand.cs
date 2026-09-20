namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to replace an existing card (due to expiry, damage, etc.)
/// </summary>
public sealed record ReplaceCardCommand(
    Guid CardId,
    string Reason = "") : IRequest<Card>;
