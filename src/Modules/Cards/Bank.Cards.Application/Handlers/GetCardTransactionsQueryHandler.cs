namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for GetCardTransactionsQuery
/// </summary>
public class GetCardTransactionsQueryHandler : IRequestHandler<GetCardTransactionsQuery, IEnumerable<CardTransactionDto>>
{
    private readonly ICardTransactionRepository _transactionRepository;

    public GetCardTransactionsQueryHandler(ICardTransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<CardTransactionDto>> Handle(GetCardTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByCardIdAsync(
            request.CardId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return transactions.Select(MapToDto).ToList();
    }

    private static CardTransactionDto MapToDto(CardTransaction transaction)
    {
        return new CardTransactionDto(
            transaction.Id,
            transaction.CardId,
            transaction.Amount,
            transaction.Type,
            transaction.Status,
            transaction.IsInternational,
            transaction.IsContactless,
            transaction.Merchant,
            transaction.TransactionDateUtc,
            transaction.PostingDateUtc);
    }
}
