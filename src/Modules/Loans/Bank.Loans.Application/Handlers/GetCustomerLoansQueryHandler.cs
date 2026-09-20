namespace Bank.Loans.Application.Handlers;

public class GetCustomerLoansQueryHandler : IRequestHandler<GetCustomerLoansQuery, PaginatedLoansDto>
{
    private readonly ILoanRepository _loanRepo;

    public GetCustomerLoansQueryHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<PaginatedLoansDto> Handle(GetCustomerLoansQuery request, CancellationToken cancellationToken)
    {
        var loans = await _loanRepo.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        var totalCount = loans.Count;

        var pagedLoans = loans
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LoanDetailDto(
                l.Id,
                l.LoanNumber,
                l.CustomerId,
                l.LoanProductId,
                l.LoanAmount,
                l.EMIAmount,
                l.TenureMonths,
                l.AnnualInterestRate,
                l.Status,
                0,
                l.ApplicationDateUtc,
                l.ApprovalDateUtc,
                l.DisbursementDateUtc,
                l.ClosureDateUtc,
                l.OutstandingPrincipal,
                l.OutstandingInterest,
                l.TotalPrincipalRepaid,
                l.TotalInterestRepaid))
            .ToList();

        return new PaginatedLoansDto(pagedLoans, totalCount, request.PageNumber, request.PageSize);
    }
}
