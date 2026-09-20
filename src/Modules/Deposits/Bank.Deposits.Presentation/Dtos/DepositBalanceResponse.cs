namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Response DTO for deposit balance query
/// </summary>
public record DepositBalanceResponse(
    Guid DepositId,
    string AccountNumber,
    decimal CurrentBalance,
    decimal AccruedInterest,
    decimal PaidInterest,
    decimal TotalBalance);
