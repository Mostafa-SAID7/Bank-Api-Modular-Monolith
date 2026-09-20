namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Request DTO for creating a new deposit account
/// </summary>
public record CreateDepositRequest(
    Guid CustomerId,
    Guid DepositTypeId,
    decimal InitialDeposit,
    bool IsFixedDeposit,
    int? TermMonths);
