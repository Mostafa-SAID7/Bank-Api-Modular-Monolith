namespace Bank.Loans.Application.Commands;

public record DisburseLoansCommand(
    Guid LoanId,
    string? ReferenceNumber = null) : IRequest<Loan>;
