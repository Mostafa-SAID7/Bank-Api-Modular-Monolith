namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for CloseCardCommand
/// </summary>
public class CloseCardCommandHandler : IRequestHandler<CloseCardCommand, Card>
{
    private readonly ICardRepository _cardRepository;

    public CloseCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Card> Handle(CloseCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.Close(request.Reason);
        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
