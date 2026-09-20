namespace Bank.Loans.Presentation.Responses;

/// <summary>
/// Response containing loan details
/// </summary>
public record LoanResponse(
    Guid Id,
    string LoanNumber,
    Guid CustomerId,
    Guid LoanProductId,
    decimal LoanAmount,
    decimal ProcessingFee,
    decimal NetDisbursedAmount,
    int TenureMonths,
    decimal AnnualInterestRate,
    int InterestType,
    int EMIFrequency,
    decimal EMIAmount,
    int Status,
    DateTime ApplicationDateUtc,
    DateTime? ApprovalDateUtc,
    DateTime? DisbursementDateUtc,
    DateTime? ClosureDateUtc,
    string? ApprovalNotes,
    Guid? ApprovedByUserId,
    decimal TotalPrincipalRepaid,
    decimal TotalInterestRepaid,
    decimal TotalPenaltyCharges,
    decimal OutstandingPrincipal,
    decimal OutstandingInterest,
    string? CollateralDescription,
    decimal? CollateralValue,
    decimal? PrepaymentPenaltyPercent,
    DateTime UpdatedAtUtc);
