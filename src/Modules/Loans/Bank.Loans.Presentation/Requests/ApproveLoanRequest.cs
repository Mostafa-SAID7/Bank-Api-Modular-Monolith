namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to approve a loan application
/// </summary>
public record ApproveLoanRequest(
    Guid LoanId,
    Guid ApprovedByUserId,
    string? ApprovalNotes = null);
