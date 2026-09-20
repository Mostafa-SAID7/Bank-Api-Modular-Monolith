namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to apply for a new loan
/// </summary>
public record CreateLoanRequest(
    Guid CustomerId,
    Guid LoanProductId,
    decimal LoanAmount,
    int TenureMonths,
    string? CollateralDescription = null,
    decimal? CollateralValue = null);
