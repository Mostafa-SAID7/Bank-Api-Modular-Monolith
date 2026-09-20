namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for GetCardQuery
/// </summary>
public class GetCardQueryHandler : IRequestHandler<GetCardQuery, CardDto>
{
    private readonly ICardRepository _cardRepository;

    public GetCardQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<CardDto> Handle(GetCardQuery request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetByIdAsync(request.CardId, cancellationToken);
        if (card == null)
            throw new InvalidOperationException("Card not found");

        return MapToDto(card);
    }

    private static CardDto MapToDto(Card card)
    {
        return new CardDto(
            card.Id,
            card.CustomerId,
            card.LinkedAccountId,
            card.CardProductId,
            card.MaskedPan,
            card.CardType,
            card.CardBrand,
            card.Status,
            card.HolderName,
            card.IssuedDateUtc,
            card.ExpiryDateUtc,
            card.ActivatedDateUtc,
            card.BlockedDateUtc,
            card.ClosedDateUtc,
            card.DailyWithdrawalLimit,
            card.DailyTransactionLimit,
            card.DailyForeignTransactionLimit,
            card.InternationalEnabled,
            card.OnlineTransactionsEnabled,
            card.ContactlessEnabled,
            card.CreatedAtUtc,
            card.UpdatedAtUtc);
    }
}
