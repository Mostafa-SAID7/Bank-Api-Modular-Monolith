namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for IssueCardCommand
/// </summary>
public class IssueCardCommandHandler : IRequestHandler<IssueCardCommand, Card>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardProductRepository _cardProductRepository;
    private readonly ICardNumberGenerator _cardNumberGenerator;
    private readonly ICardEncryptionService _encryptionService;

    public IssueCardCommandHandler(
        ICardRepository cardRepository,
        ICardProductRepository cardProductRepository,
        ICardNumberGenerator cardNumberGenerator,
        ICardEncryptionService encryptionService)
    {
        _cardRepository = cardRepository;
        _cardProductRepository = cardProductRepository;
        _cardNumberGenerator = cardNumberGenerator;
        _encryptionService = encryptionService;
    }

    public async Task<Card> Handle(IssueCardCommand request, CancellationToken cancellationToken)
    {
        var product = await _cardProductRepository.GetByIdAsync(request.CardProductId, cancellationToken);
        if (product == null || !product.IsActive)
            throw new InvalidOperationException("Card product not found or inactive");

        var cardNumber = await _cardNumberGenerator.GenerateAsync(cancellationToken);
        
        // Generate a random CVV for demo purposes - in production, would be handled securely
        var cvv = Random.Shared.Next(100, 999).ToString();

        var card = Card.Issue(
            request.CustomerId,
            request.LinkedAccountId,
            request.CardProductId,
            product,
            request.HolderName,
            cardNumber,
            cvv);

        await _cardRepository.AddAsync(card, cancellationToken);
        return card;
    }
}
