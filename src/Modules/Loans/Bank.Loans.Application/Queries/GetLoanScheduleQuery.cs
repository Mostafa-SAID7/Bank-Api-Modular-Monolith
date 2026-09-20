namespace Bank.Loans.Application.Queries;

public record GetLoanScheduleQuery(Guid LoanId) : IRequest<List<LoanScheduleDto>>;
