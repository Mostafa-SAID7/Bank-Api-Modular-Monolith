using Bank.CoreBanking.Application.Queries;

namespace Bank.CoreBanking.Application.Handlers;

/// <summary>
/// Handles GetTransactionByIdQuery
/// Retrieves transaction details by transaction ID
/// </summary>
public sealed class GetTransactionByIdQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<GetTransactionByIdResult?> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(query.TransactionId, cancellationToken);
        
        if (transaction is null)
            return null;

        return new GetTransactionByIdResult(
            transaction.Id,
            transaction.FromAccountId,
            transaction.ToAccountId,
            transaction.Amount,
            transaction.Currency,
            (int)transaction.Status,
            (int)transaction.Type,
            transaction.Reference,
            transaction.Description,
            transaction.CreatedAtUtc,
            transaction.CompletedAtUtc,
            transaction.FailureReason
        );
    }
}
