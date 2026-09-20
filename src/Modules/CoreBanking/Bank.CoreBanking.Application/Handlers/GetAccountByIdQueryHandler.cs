using Bank.CoreBanking.Application.Queries;

namespace Bank.CoreBanking.Application.Handlers;

/// <summary>
/// Handles GetAccountByIdQuery
/// Retrieves account details by account ID
/// </summary>
public sealed class GetAccountByIdQueryHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountByIdResult?> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(query.AccountId, cancellationToken);
        
        if (account is null)
            return null;

        return new GetAccountByIdResult(
            account.Id,
            account.AccountNumber,
            account.AccountHolderName,
            account.CustomerId,
            account.Balance,
            account.Currency,
            (int)account.Status,
            (int)account.Type,
            account.OpenedAtUtc,
            account.LastActivityDate
        );
    }
}
