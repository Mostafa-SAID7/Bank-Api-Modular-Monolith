using Bank.CoreBanking.Application.Queries;

namespace Bank.CoreBanking.Application.Handlers;

/// <summary>
/// Handles GetAccountTransactionsQuery
/// Retrieves all transactions for an account with pagination support
/// </summary>
public sealed class GetAccountTransactionsQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetAccountTransactionsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<GetAccountTransactionsResult> Handle(GetAccountTransactionsQuery query, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetByAccountAsync(query.AccountId, cancellationToken);

        // Apply pagination
        var totalCount = transactions.Count;
        var pagedTransactions = transactions
            .OrderByDescending(t => t.CreatedAtUtc) // Most recent first
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => new TransactionSummary(
                t.Id,
                t.FromAccountId,
                t.ToAccountId,
                t.Amount,
                t.Currency,
                (int)t.Status,
                (int)t.Type,
                t.Reference,
                t.Description,
                t.CreatedAtUtc,
                t.CompletedAtUtc
            ))
            .ToList();

        return new GetAccountTransactionsResult(
            pagedTransactions,
            query.PageNumber,
            query.PageSize,
            totalCount
        );
    }
}
