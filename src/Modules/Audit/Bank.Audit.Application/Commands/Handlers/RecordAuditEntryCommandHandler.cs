namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;

internal sealed class RecordAuditEntryCommandHandler : IRequestHandler<RecordAuditEntryCommand, Guid>
{
    private readonly IAuditEntryRepository _auditEntryRepository;

    public RecordAuditEntryCommandHandler(IAuditEntryRepository auditEntryRepository)
    {
        _auditEntryRepository = auditEntryRepository;
    }

    public async Task<Guid> Handle(RecordAuditEntryCommand request, CancellationToken cancellationToken)
    {
        var auditEntry = AuditEntry.Create(
            request.AuditLogId,
            request.EntityName,
            request.ActionType,
            request.OldValues,
            request.NewValues,
            request.ChangeType);

        await _auditEntryRepository.AddAsync(auditEntry, cancellationToken);
        return auditEntry.Id;
    }
}
