namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for BlockCardCommand
/// </summary>
public class BlockCardCommandHandler : IRequestHandler<BlockCardCommand, Card>
{
    private readonly ICardRepository _cardRepository;

    public BlockCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Card> Handle(BlockCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.Block(request.Reason, request.Description);

        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
