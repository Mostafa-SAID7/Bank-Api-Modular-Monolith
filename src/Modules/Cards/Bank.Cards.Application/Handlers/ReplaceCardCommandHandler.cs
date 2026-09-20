namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for ReplaceCardCommand
/// </summary>
public class ReplaceCardCommandHandler : IRequestHandler<ReplaceCardCommand, Card>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardProductRepository _cardProductRepository;
    private readonly ICardNumberGenerator _cardNumberGenerator;

    public ReplaceCardCommandHandler(
        ICardRepository cardRepository,
        ICardProductRepository cardProductRepository,
        ICardNumberGenerator cardNumberGenerator)
    {
        _cardRepository = cardRepository;
        _cardProductRepository = cardProductRepository;
        _cardNumberGenerator = cardNumberGenerator;
    }

    public async Task<Card> Handle(ReplaceCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        var product = await _cardProductRepository.GetByIdAsync(card.CardProductId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException("Card product not found");

        var newCardNumber = await _cardNumberGenerator.GenerateAsync(cancellationToken);
        var newCvv = Random.Shared.Next(100, 999).ToString();

        var replacementCard = card.Replace(product, newCardNumber, newCvv);

        await _cardRepository.UpdateAsync(card, cancellationToken);
        await _cardRepository.AddAsync(replacementCard, cancellationToken);
        
        return replacementCard;
    }
}
