using Bank.CoreBanking.Application.Queries;

namespace Bank.CoreBanking.Application.Handlers;

/// <summary>
/// Handles GetCustomerAccountsQuery
/// Retrieves all accounts for a customer with pagination support
/// </summary>
public sealed class GetCustomerAccountsQueryHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetCustomerAccountsQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetCustomerAccountsResult> Handle(GetCustomerAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);

        // Apply pagination
        var totalCount = accounts.Count;
        var pagedAccounts = accounts
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new AccountSummary(
                a.Id,
                a.AccountNumber,
                a.AccountHolderName,
                a.Balance,
                a.Currency,
                (int)a.Status,
                (int)a.Type,
                a.OpenedAtUtc,
                a.LastActivityDate
            ))
            .ToList();

        return new GetCustomerAccountsResult(
            pagedAccounts,
            query.PageNumber,
            query.PageSize,
            totalCount
        );
    }
}
