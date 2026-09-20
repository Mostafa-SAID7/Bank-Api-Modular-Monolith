namespace Bank.Audit.Application.Queries.Handlers;

using Bank.Audit.Application.DTOs;
using Bank.Audit.Domain.Repositories;

internal sealed class GetAuditStatisticsQueryHandler : IRequestHandler<GetAuditStatisticsQuery, AuditStatisticsDto>
{
    private readonly IAuditQueryService _auditQueryService;

    public GetAuditStatisticsQueryHandler(IAuditQueryService auditQueryService)
    {
        _auditQueryService = auditQueryService;
    }

    public async Task<AuditStatisticsDto> Handle(GetAuditStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _auditQueryService.GetStatisticsAsync(cancellationToken);
    }
}
