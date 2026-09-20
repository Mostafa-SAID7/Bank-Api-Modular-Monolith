namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Response DTO for deposit creation
/// </summary>
public record CreateDepositResponse(
    Guid DepositId,
    string AccountNumber,
    Guid CustomerId,
    decimal CurrentBalance,
    int Status,
    DateTime OpenedAtUtc,
    DateTime? MaturityDateUtc,
    bool IsFixedDeposit);
