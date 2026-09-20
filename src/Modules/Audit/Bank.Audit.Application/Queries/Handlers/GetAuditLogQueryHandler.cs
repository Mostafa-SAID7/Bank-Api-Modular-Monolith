namespace Bank.Audit.Application.Queries.Handlers;

using Bank.Audit.Application.DTOs;
using Bank.Audit.Domain.Repositories;

internal sealed class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, AuditLogDto?>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogDto?> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(request.AuditLogId, cancellationToken);
        if (auditLog is null)
            return null;

        return new AuditLogDto(
            auditLog.Id,
            auditLog.UserId,
            auditLog.EventType,
            auditLog.ResourceType,
            auditLog.ResourceId,
            auditLog.Action,
            auditLog.Timestamp,
            auditLog.IpAddress,
            auditLog.UserAgent,
            auditLog.Status,
            auditLog.Details);
    }
}
