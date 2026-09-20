namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for GetCardProductsQuery
/// </summary>
public class GetCardProductsQueryHandler : IRequestHandler<GetCardProductsQuery, IEnumerable<CardProductDto>>
{
    private readonly ICardProductRepository _cardProductRepository;

    public GetCardProductsQueryHandler(ICardProductRepository cardProductRepository)
    {
        _cardProductRepository = cardProductRepository;
    }

    public async Task<IEnumerable<CardProductDto>> Handle(GetCardProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _cardProductRepository.GetAllActiveAsync(cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    private static CardProductDto MapToDto(CardProduct product)
    {
        return new CardProductDto(
            product.Id,
            product.Name,
            product.CardType,
            product.Brand,
            product.DefaultDailyLimit,
            product.DefaultTransactionLimit,
            product.RequiresPinForAtm,
            product.CardValidityYears,
            product.IsActive);
    }
}
