namespace Bank.CoreBanking.Application.Queries;

/// <summary>
/// Query to retrieve all transactions for an account with pagination
/// </summary>
public sealed record GetAccountTransactionsQuery(
    Guid AccountId,
    int PageNumber = 1,
    int PageSize = 20
);

/// <summary>
/// Paginated result of account transactions
/// </summary>
public sealed record GetAccountTransactionsResult(
    IReadOnlyList<TransactionSummary> Transactions,
    int PageNumber,
    int PageSize,
    int TotalCount
)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}

/// <summary>
/// Transaction summary for list responses
/// </summary>
public sealed record TransactionSummary(
    Guid Id,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string Currency,
    int Status,
    int Type,
    string? Reference,
    string? Description,
    DateTime InitiatedAtUtc,
    DateTime? CompletedAtUtc
);
