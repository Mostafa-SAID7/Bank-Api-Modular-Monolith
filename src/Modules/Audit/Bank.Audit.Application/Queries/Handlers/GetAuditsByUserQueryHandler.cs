namespace Bank.Audit.Application.Queries.Handlers;

using Bank.Audit.Application.DTOs;
using Bank.Audit.Domain.Repositories;

internal sealed class GetAuditsByUserQueryHandler : IRequestHandler<GetAuditsByUserQuery, IEnumerable<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditsByUserQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<AuditLogDto>> Handle(GetAuditsByUserQuery request, CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return auditLogs.Select(a => new AuditLogDto(
            a.Id,
            a.UserId,
            a.EventType,
            a.ResourceType,
            a.ResourceId,
            a.Action,
            a.Timestamp,
            a.IpAddress,
            a.UserAgent,
            a.Status,
            a.Details));
    }
}
