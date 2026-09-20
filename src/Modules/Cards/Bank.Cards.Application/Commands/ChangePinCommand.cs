namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to change the PIN of a card
/// </summary>
public sealed record ChangePinCommand(
    Guid CardId,
    string CurrentPin,
    string NewPin) : IRequest<Card>;
