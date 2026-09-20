namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for ChangePinCommand
/// </summary>
public class ChangePinCommandHandler : IRequestHandler<ChangePinCommand, Card>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardEncryptionService _encryptionService;

    public ChangePinCommandHandler(
        ICardRepository cardRepository,
        ICardEncryptionService encryptionService)
    {
        _cardRepository = cardRepository;
        _encryptionService = encryptionService;
    }

    public async Task<Card> Handle(ChangePinCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        // Validate current PIN
        card.ValidatePin(request.CurrentPin);

        // Change to new PIN
        card.ChangePin(request.NewPin);

        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
