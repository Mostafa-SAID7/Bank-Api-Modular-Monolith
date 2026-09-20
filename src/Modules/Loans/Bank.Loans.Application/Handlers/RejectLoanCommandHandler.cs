namespace Bank.Loans.Application.Handlers;

public class RejectLoanCommandHandler : IRequestHandler<RejectLoanCommand, Loan>
{
    private readonly ILoanRepository _loanRepo;

    public RejectLoanCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<Loan> Handle(RejectLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            throw new KeyNotFoundException($"Loan {request.LoanId} not found");

        loan.Reject(request.Reason);
        await _loanRepo.UpdateAsync(loan, cancellationToken);
        return loan;
    }
}
