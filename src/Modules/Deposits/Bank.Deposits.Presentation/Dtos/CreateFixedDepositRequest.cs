namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Request DTO for creating a new fixed deposit
/// </summary>
public record CreateFixedDepositRequest(
    Guid CustomerId,
    Guid DepositProductId,
    decimal PrincipalAmount,
    int TermMonths,
    DateTime StartDate,
    bool EnableAutoRenewal,
    int? InterestCalculationMethod = null);
