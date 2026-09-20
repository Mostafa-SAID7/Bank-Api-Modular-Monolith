namespace Bank.Audit.Application.Commands;

using Bank.Audit.Domain.Enums;

public record RecordAuditEntryCommand(
    Guid AuditLogId,
    string EntityName,
    ActionType ActionType,
    string OldValues,
    string NewValues,
    EntityChangeType ChangeType) : IRequest<Guid>;
