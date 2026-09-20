namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Request DTO for freezing a deposit account
/// </summary>
public record FreezeDepositRequest(
    Guid DepositId,
    string Reason);
