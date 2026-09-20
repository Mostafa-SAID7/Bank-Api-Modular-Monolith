namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Repositories;

internal sealed class ArchiveAuditLogCommandHandler : IRequestHandler<ArchiveAuditLogCommand, bool>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public ArchiveAuditLogCommandHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<bool> Handle(ArchiveAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(request.AuditLogId, cancellationToken);
        if (auditLog is null)
            return false;

        auditLog.Archive();
        // Note: Add UpdateAsync to repository if needed
        return true;
    }
}
