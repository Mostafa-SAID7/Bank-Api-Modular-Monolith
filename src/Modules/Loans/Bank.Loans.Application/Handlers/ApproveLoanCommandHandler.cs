namespace Bank.Loans.Application.Handlers;

public class ApproveLoanCommandHandler : IRequestHandler<ApproveLoanCommand, Loan>
{
    private readonly ILoanRepository _loanRepo;

    public ApproveLoanCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<Loan> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            throw new KeyNotFoundException($"Loan {request.LoanId} not found");

        loan.Approve(request.ApprovedByUserId, request.Notes);
        await _loanRepo.UpdateAsync(loan, cancellationToken);
        return loan;
    }
}
