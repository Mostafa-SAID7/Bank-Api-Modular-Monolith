using Bank.Deposits.Application.Interfaces;
using Bank.Deposits.Application.Queries;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// DTO for deposit query response
/// </summary>
public class DepositDetailDto
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AccruedInterest { get; set; }
    public decimal PaidInterest { get; set; }
    public int Status { get; set; }
    public DateTime OpenedAtUtc { get; set; }
    public DateTime? MaturityDateUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public bool IsFixedDeposit { get; set; }
    public int TermMonths { get; set; }
}

/// <summary>
/// Handler for GetDepositByIdQuery
/// </summary>
public class GetDepositByIdQueryHandler : IQueryHandler<GetDepositByIdQuery, DepositDetailDto?>
{
    private readonly IDepositRepository _depositRepository;

    public GetDepositByIdQueryHandler(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task<DepositDetailDto?> Handle(GetDepositByIdQuery query, CancellationToken cancellationToken)
    {
        var deposit = await _depositRepository.GetByIdAsync(query.DepositId, cancellationToken);
        if (deposit is null)
            return null;

        return new DepositDetailDto
        {
            Id = deposit.Id,
            AccountNumber = deposit.AccountNumber,
            CustomerId = deposit.CustomerId,
            CurrentBalance = deposit.CurrentBalance,
            AccruedInterest = deposit.AccruedInterest,
            PaidInterest = deposit.PaidInterest,
            Status = (int)deposit.Status,
            OpenedAtUtc = deposit.OpenedAtUtc,
            MaturityDateUtc = deposit.MaturityDateUtc,
            ClosedAtUtc = deposit.ClosedAtUtc,
            IsFixedDeposit = deposit.IsFixedDeposit,
            TermMonths = deposit.TermMonths
        };
    }
}
