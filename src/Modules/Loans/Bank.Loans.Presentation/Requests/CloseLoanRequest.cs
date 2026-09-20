namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to close a loan
/// </summary>
public record CloseLoanRequest(
    Guid LoanId);
