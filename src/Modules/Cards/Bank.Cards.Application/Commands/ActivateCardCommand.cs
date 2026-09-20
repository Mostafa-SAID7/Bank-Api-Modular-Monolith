namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to activate a card
/// </summary>
public sealed record ActivateCardCommand(
    Guid CardId,
    string Pin) : IRequest<Card>;
