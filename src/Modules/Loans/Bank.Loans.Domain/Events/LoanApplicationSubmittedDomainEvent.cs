namespace Bank.Loans.Domain.Events;

public record LoanApplicationSubmittedDomainEvent(
    Guid LoanId,
    Guid CustomerId,
    string LoanNumber,
    decimal LoanAmount,
    int TenureMonths) : DomainEvent(LoanId, "LoanApplicationSubmitted");
