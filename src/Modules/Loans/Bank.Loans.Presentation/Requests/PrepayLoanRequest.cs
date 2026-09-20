namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to prepay a loan (partial or full)
/// </summary>
public record PrepayLoanRequest(
    Guid LoanId,
    decimal PrepaymentAmount);
