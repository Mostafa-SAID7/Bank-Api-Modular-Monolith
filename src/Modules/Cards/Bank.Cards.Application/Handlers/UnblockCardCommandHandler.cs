namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for UnblockCardCommand
/// </summary>
public class UnblockCardCommandHandler : IRequestHandler<UnblockCardCommand, Card>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardBlockRepository _blockRepository;

    public UnblockCardCommandHandler(
        ICardRepository cardRepository,
        ICardBlockRepository blockRepository)
    {
        _cardRepository = cardRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Card> Handle(UnblockCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.Unblock();
        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
