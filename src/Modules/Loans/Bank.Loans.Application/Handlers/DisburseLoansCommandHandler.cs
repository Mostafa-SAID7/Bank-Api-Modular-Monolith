namespace Bank.Loans.Application.Handlers;

public class DisburseLoansCommandHandler : IRequestHandler<DisburseLoansCommand, Loan>
{
    private readonly ILoanRepository _loanRepo;

    public DisburseLoansCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<Loan> Handle(DisburseLoansCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            throw new KeyNotFoundException($"Loan {request.LoanId} not found");

        loan.Disburse(request.ReferenceNumber);
        await _loanRepo.UpdateAsync(loan, cancellationToken);
        return loan;
    }
}
