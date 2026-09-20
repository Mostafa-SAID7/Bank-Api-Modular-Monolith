namespace Bank.CoreBanking.Presentation.Dtos;

/// <summary>
/// Response DTO for transaction details
/// </summary>
public sealed record TransactionResponse(
    Guid Id,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string Currency,
    int Status,
    int Type,
    string? Reference,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc,
    string? FailureReason
);
