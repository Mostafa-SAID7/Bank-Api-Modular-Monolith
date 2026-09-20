namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to enable or disable international transactions on a card
/// </summary>
public sealed record UpdateInternationalStatusCommand(
    Guid CardId,
    bool Enabled) : IRequest<Card>;
