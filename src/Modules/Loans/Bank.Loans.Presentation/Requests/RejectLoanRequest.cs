namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to reject a loan application
/// </summary>
public record RejectLoanRequest(
    Guid LoanId,
    string RejectionReason);
