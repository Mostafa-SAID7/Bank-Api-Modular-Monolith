namespace Bank.Loans.Application.Handlers;

public class GetLoanByIdQueryHandler : IRequestHandler<GetLoanByIdQuery, LoanDetailDto?>
{
    private readonly ILoanRepository _loanRepo;

    public GetLoanByIdQueryHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<LoanDetailDto?> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            return null;

        return new LoanDetailDto(
            loan.Id,
            loan.LoanNumber,
            loan.CustomerId,
            loan.LoanProductId,
            loan.LoanAmount,
            loan.EMIAmount,
            loan.TenureMonths,
            loan.AnnualInterestRate,
            loan.Status,
            0, // LoanType would be retrieved from product
            loan.ApplicationDateUtc,
            loan.ApprovalDateUtc,
            loan.DisbursementDateUtc,
            loan.ClosureDateUtc,
            loan.OutstandingPrincipal,
            loan.OutstandingInterest,
            loan.TotalPrincipalRepaid,
            loan.TotalInterestRepaid);
    }
}
