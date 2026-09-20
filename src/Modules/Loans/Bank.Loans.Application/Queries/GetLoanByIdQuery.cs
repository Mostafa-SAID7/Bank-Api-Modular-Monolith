namespace Bank.Loans.Application.Queries;

public record GetLoanByIdQuery(Guid LoanId) : IRequest<LoanDetailDto?>;
