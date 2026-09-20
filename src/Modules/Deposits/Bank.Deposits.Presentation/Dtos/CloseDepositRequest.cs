namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Request DTO for closing a deposit account
/// </summary>
public record CloseDepositRequest(
    Guid DepositId,
    string Reason);
