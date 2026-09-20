namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for UpdateInternationalStatusCommand
/// </summary>
public class UpdateInternationalStatusCommandHandler : IRequestHandler<UpdateInternationalStatusCommand, Card>
{
    private readonly ICardRepository _cardRepository;

    public UpdateInternationalStatusCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Card> Handle(UpdateInternationalStatusCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        card.UpdateInternationalStatus(request.Enabled);
        await _cardRepository.UpdateAsync(card, cancellationToken);
        return card;
    }
}
