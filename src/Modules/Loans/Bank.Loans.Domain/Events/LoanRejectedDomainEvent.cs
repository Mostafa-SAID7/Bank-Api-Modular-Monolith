namespace Bank.Loans.Domain.Events;

public record LoanRejectedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    string RejectionReason) : DomainEvent(LoanId, "LoanRejected");
