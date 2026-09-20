namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for UpdateCardLimitsCommand
/// </summary>
public class UpdateCardLimitsCommandHandler : IRequestHandler<UpdateCardLimitsCommand, Card>
{
    private readonly ICardRepository _cardRepository;

    public UpdateCardLimitsCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Card> Handle(UpdateCardLimitsCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.UpdateLimits(
            request.DailyWithdrawalLimit,
            request.DailyTransactionLimit,
            request.DailyForeignTransactionLimit);

        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
