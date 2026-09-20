namespace Bank.Loans.Application.Commands;

public record ApproveLoanCommand(
    Guid LoanId,
    Guid ApprovedByUserId,
    string? Notes = null) : IRequest<Loan>;
