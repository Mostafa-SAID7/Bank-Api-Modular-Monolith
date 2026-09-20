namespace Bank.Loans.Domain.Events;

public record LoanDefaultedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal OutstandingAmount,
    string DefaultReason) : DomainEvent(LoanId, "LoanDefaulted");
