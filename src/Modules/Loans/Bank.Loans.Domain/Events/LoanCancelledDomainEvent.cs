namespace Bank.Loans.Domain.Events;

public record LoanCancelledDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    string CancellationReason) : DomainEvent(LoanId, "LoanCancelled");
