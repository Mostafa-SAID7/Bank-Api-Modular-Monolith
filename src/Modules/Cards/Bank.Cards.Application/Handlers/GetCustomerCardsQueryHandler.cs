namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for GetCustomerCardsQuery
/// </summary>
public class GetCustomerCardsQueryHandler : IRequestHandler<GetCustomerCardsQuery, IEnumerable<CardDto>>
{
    private readonly ICardRepository _cardRepository;

    public GetCustomerCardsQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<IEnumerable<CardDto>> Handle(GetCustomerCardsQuery request, CancellationToken cancellationToken)
    {
        var cards = await _cardRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return cards.Select(MapToDto).ToList();
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
