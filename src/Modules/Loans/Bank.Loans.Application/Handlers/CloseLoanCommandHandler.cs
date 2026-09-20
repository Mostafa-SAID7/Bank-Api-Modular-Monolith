namespace Bank.Loans.Application.Handlers;

public class CloseLoanCommandHandler : IRequestHandler<CloseLoanCommand, Loan>
{
    private readonly ILoanRepository _loanRepo;

    public CloseLoanCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<Loan> Handle(CloseLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            throw new KeyNotFoundException($"Loan {request.LoanId} not found");

        loan.Close();
        await _loanRepo.UpdateAsync(loan, cancellationToken);
        return loan;
    }
}
