namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to disburse an approved loan
/// </summary>
public record DisburseLoanRequest(
    Guid LoanId,
    string? DisbursementReferenceNumber = null);
