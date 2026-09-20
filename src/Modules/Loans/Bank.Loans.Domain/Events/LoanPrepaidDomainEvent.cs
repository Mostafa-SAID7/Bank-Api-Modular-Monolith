namespace Bank.Loans.Domain.Events;

public record LoanPrepaidDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal PrepaidAmount,
    decimal PenaltyCharge,
    decimal RemainingOutstanding) : DomainEvent(LoanId, "LoanPrepaid");
