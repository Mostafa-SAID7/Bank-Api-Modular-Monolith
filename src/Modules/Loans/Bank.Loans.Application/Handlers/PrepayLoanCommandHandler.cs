namespace Bank.Loans.Application.Handlers;

public class PrepayLoanCommandHandler : IRequestHandler<PrepayLoanCommand, Loan>
{
    private readonly ILoanRepository _loanRepo;

    public PrepayLoanCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<Loan> Handle(PrepayLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            throw new KeyNotFoundException($"Loan {request.LoanId} not found");

        loan.Prepay(request.Amount);
        await _loanRepo.UpdateAsync(loan, cancellationToken);
        return loan;
    }
}
