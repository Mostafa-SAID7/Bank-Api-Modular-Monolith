namespace Bank.Loans.Domain.Events;

public record EMIPaymentRecordedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal PrincipalPaid,
    decimal InterestPaid,
    decimal OutstandingPrincipal) : DomainEvent(LoanId, "EMIPaymentRecorded");
