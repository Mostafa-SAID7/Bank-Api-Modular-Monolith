namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for ActivateCardCommand
/// </summary>
public class ActivateCardCommandHandler : IRequestHandler<ActivateCardCommand, Card>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardEncryptionService _encryptionService;

    public ActivateCardCommandHandler(
        ICardRepository cardRepository,
        ICardEncryptionService encryptionService)
    {
        _cardRepository = cardRepository;
        _encryptionService = encryptionService;
    }

    public async Task<Card> Handle(ActivateCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.Activate();
        
        // Set PIN during activation
        var pinHash = _encryptionService.HashPin(request.Pin);
        card.GetType().GetProperty("PinHash")?.SetValue(card, pinHash);
        card.GetType().GetProperty("PinRequired")?.SetValue(card, true);

        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
