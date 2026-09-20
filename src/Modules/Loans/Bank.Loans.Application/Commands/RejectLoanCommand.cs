namespace Bank.Loans.Application.Commands;

public record RejectLoanCommand(
    Guid LoanId,
    string Reason) : IRequest<Loan>;
