namespace Bank.Loans.Domain.Events;

public record LoanClosedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal TotalPrincipalRepaid,
    decimal TotalInterestRepaid,
    decimal TotalPenaltyCharges) : DomainEvent(LoanId, "LoanClosed");
