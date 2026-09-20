namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Request DTO for deposit transactions (deposit/withdrawal)
/// </summary>
public record DepositTransactionRequest(
    Guid DepositId,
    decimal Amount,
    string Description);
