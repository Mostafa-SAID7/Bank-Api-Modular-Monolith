namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for RecordTransactionCommand
/// </summary>
public class RecordTransactionCommandHandler : IRequestHandler<RecordTransactionCommand, Unit>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICardTransactionRepository _transactionRepository;

    public RecordTransactionCommandHandler(
        ICardRepository cardRepository,
        ICardTransactionRepository transactionRepository)
    {
        _cardRepository = cardRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Unit> Handle(RecordTransactionCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        // Validate card can process transaction
        card.ValidateForTransaction(
            request.Amount,
            request.Type,
            request.IsInternational);

        // Record transaction against limits
        card.RecordTransaction(
            request.Amount,
            request.Type,
            request.IsInternational);

        // Create transaction record
        var transaction = CardTransaction.Create(
            request.CardId,
            request.Type,
            request.Amount,
            "Point of Sale",
            "Retail",
            request.IsInternational);

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _cardRepository.UpdateAsync(card, cancellationToken);
        
        return Unit.Value;
    }
}
