namespace Bank.CoreBanking.Application.Queries;

/// <summary>
/// Query to retrieve an account by its ID
/// </summary>
public sealed record GetAccountByIdQuery(Guid AccountId);

/// <summary>
/// Result of retrieving an account
/// </summary>
public sealed record GetAccountByIdResult(
    Guid Id,
    string AccountNumber,
    string AccountHolderName,
    Guid CustomerId,
    decimal Balance,
    string Currency,
    int Status,
    int Type,
    DateTime OpenedAtUtc,
    DateTime LastActivityDate
);
