namespace Bank.CoreBanking.Application.Queries;

/// <summary>
/// Query to retrieve a transaction by its ID
/// </summary>
public sealed record GetTransactionByIdQuery(Guid TransactionId);

/// <summary>
/// Result of retrieving a transaction
/// </summary>
public sealed record GetTransactionByIdResult(
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
    DateTime? CompletedAtUtc,
    string? FailureReason
);
