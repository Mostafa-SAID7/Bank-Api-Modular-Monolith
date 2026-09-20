namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Application.DTOs;
using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;

internal sealed class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, Guid>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public CreateAuditLogCommandHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditLog = AuditLog.Create(
            request.UserId,
            request.EventType,
            request.ResourceType,
            request.ResourceId,
            request.Action,
            request.Timestamp,
            request.IpAddress,
            request.UserAgent,
            request.Details);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        return auditLog.Id;
    }
}
