namespace Bank.Loans.Domain.Events;

public record LoanActivatedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber) : DomainEvent(LoanId, "LoanActivated");
