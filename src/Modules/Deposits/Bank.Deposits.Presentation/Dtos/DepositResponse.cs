namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Response DTO for deposit detail
/// </summary>
public record DepositResponse(
    Guid Id,
    string AccountNumber,
    Guid CustomerId,
    decimal CurrentBalance,
    decimal AccruedInterest,
    decimal PaidInterest,
    int Status,
    DateTime OpenedAtUtc,
    DateTime? MaturityDateUtc,
    DateTime? ClosedAtUtc,
    bool IsFixedDeposit,
    int? TermMonths);
