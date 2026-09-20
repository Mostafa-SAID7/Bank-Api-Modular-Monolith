namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for ValidateCardForTransactionQuery
/// </summary>
public class ValidateCardForTransactionQueryHandler : IRequestHandler<ValidateCardForTransactionQuery, bool>
{
    private readonly ICardRepository _cardRepository;

    public ValidateCardForTransactionQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<bool> Handle(ValidateCardForTransactionQuery request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            return false;

        try
        {
            card.ValidateForTransaction(
                request.Amount,
                request.Type,
                request.IsInternational,
                request.IsContactless);
            
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
