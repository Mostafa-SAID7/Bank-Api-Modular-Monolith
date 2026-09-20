namespace Bank.Loans.Application.Commands;

public record PrepayLoanCommand(
    Guid LoanId,
    decimal Amount) : IRequest<Loan>;
