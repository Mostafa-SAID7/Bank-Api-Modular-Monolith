namespace Bank.Loans.Domain.Events;

public record LoanApprovedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal LoanAmount,
    Guid ApprovedByUserId) : DomainEvent(LoanId, "LoanApproved");
