namespace Bank.Loans.Domain.Events;

public record LoanDisbursedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal DisbursedAmount,
    string ReferenceNumber) : DomainEvent(LoanId, "LoanDisbursed");
