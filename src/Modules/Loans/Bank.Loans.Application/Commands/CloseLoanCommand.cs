namespace Bank.Loans.Application.Commands;

public record CloseLoanCommand(
    Guid LoanId) : IRequest<Loan>;
