namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to close a card permanently
/// </summary>
public sealed record CloseCardCommand(
    Guid CardId,
    string Reason = "") : IRequest<Card>;
