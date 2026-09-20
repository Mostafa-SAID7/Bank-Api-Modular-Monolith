namespace Bank.Deposits.Presentation.Dtos;

/// <summary>
/// Request DTO for processing maturity of a fixed deposit
/// </summary>
public record ProcessMaturityRequest(
    Guid FixedDepositId,
    int MaturityAction,
    Guid? RenewalProductId = null,
    int? RenewalTermMonths = null);
