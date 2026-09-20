using Bank.Deposits.Application.Interfaces;
using Bank.Deposits.Application.Queries;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// DTO for deposit balance information
/// </summary>
public class DepositBalanceDto
{
    public Guid DepositId { get; set; }
    public string AccountNumber { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public decimal AccruedInterest { get; set; }
    public decimal PaidInterest { get; set; }
    public decimal TotalBalance => CurrentBalance + AccruedInterest;
}

/// <summary>
/// Handler for GetDepositBalanceQuery
/// </summary>
public class GetDepositBalanceQueryHandler : IQueryHandler<GetDepositBalanceQuery, DepositBalanceDto?>
{
    private readonly IDepositRepository _depositRepository;

    public GetDepositBalanceQueryHandler(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task<DepositBalanceDto?> Handle(GetDepositBalanceQuery query, CancellationToken cancellationToken)
    {
        var deposit = await _depositRepository.GetByIdAsync(query.DepositId, cancellationToken);
        if (deposit is null)
            return null;

        return new DepositBalanceDto
        {
            DepositId = deposit.Id,
            AccountNumber = deposit.AccountNumber,
            CurrentBalance = deposit.CurrentBalance,
            AccruedInterest = deposit.AccruedInterest,
            PaidInterest = deposit.PaidInterest
        };
    }
}
