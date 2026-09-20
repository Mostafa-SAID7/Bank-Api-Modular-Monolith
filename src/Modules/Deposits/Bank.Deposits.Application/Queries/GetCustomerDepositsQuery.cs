namespace Bank.Deposits.Application.Queries;

/// <summary>
/// Query to retrieve all deposit accounts for a customer
/// </summary>
public sealed record GetCustomerDepositsQuery(
    Guid CustomerId,
    int PageNumber = 1,
    int PageSize = 50) : Query;
