namespace Bank.Loans.Application.Queries;

public record GetCustomerLoansQuery(
    Guid CustomerId,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedLoansDto>;
