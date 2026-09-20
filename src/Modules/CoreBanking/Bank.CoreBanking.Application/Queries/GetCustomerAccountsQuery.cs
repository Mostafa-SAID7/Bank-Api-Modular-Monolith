namespace Bank.CoreBanking.Application.Queries;

/// <summary>
/// Query to retrieve all accounts for a customer with pagination
/// </summary>
public sealed record GetCustomerAccountsQuery(
    Guid CustomerId,
    int PageNumber = 1,
    int PageSize = 10
);

/// <summary>
/// Paginated result of customer accounts
/// </summary>
public sealed record GetCustomerAccountsResult(
    IReadOnlyList<AccountSummary> Accounts,
    int PageNumber,
    int PageSize,
    int TotalCount
)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}

/// <summary>
/// Account summary for list responses
/// </summary>
public sealed record AccountSummary(
    Guid Id,
    string AccountNumber,
    string AccountHolderName,
    decimal Balance,
    string Currency,
    int Status,
    int Type,
    DateTime OpenedAtUtc,
    DateTime LastActivityDate
);
