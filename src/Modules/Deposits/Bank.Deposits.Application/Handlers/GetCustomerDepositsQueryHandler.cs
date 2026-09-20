using Bank.Deposits.Application.Interfaces;
using Bank.Deposits.Application.Queries;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// DTO for paginated deposits list
/// </summary>
public class DepositListDto
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public int Status { get; set; }
    public DateTime OpenedAtUtc { get; set; }
    public bool IsFixedDeposit { get; set; }
}

public class PaginatedDepositsDto
{
    public List<DepositListDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Handler for GetCustomerDepositsQuery
/// </summary>
public class GetCustomerDepositsQueryHandler : IQueryHandler<GetCustomerDepositsQuery, PaginatedDepositsDto>
{
    private readonly IDepositRepository _depositRepository;

    public GetCustomerDepositsQueryHandler(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task<PaginatedDepositsDto> Handle(GetCustomerDepositsQuery query, CancellationToken cancellationToken)
    {
        var deposits = await _depositRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);

        var items = deposits
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DepositListDto
            {
                Id = d.Id,
                AccountNumber = d.AccountNumber,
                CurrentBalance = d.CurrentBalance,
                Status = (int)d.Status,
                OpenedAtUtc = d.OpenedAtUtc,
                IsFixedDeposit = d.IsFixedDeposit
            })
            .ToList();

        return new PaginatedDepositsDto
        {
            Items = items,
            TotalCount = deposits.Count,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
