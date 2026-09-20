namespace Bank.Loans.Application.Handlers;

public class RecordEMIPaymentCommandHandler : IRequestHandler<RecordEMIPaymentCommand, Loan>
{
    private readonly ILoanRepository _loanRepo;

    public RecordEMIPaymentCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<Loan> Handle(RecordEMIPaymentCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            throw new KeyNotFoundException($"Loan {request.LoanId} not found");

        if (request.Amount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero");

        var emiAmount = loan.EMIAmount;
        var principalComponent = (emiAmount * (request.Amount / emiAmount)) * 0.6m; // Approximate split
        var interestComponent = request.Amount - principalComponent;

        loan.RecordPayment(principalComponent, interestComponent);
        await _loanRepo.UpdateAsync(loan, cancellationToken);
        return loan;
    }
}
