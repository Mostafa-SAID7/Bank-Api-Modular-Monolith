namespace Bank.Audit.Application.Commands;

using Bank.Audit.Domain.Enums;

public record ConfigureAuditLevelCommand(AuditLevel AuditLevel, string ConfiguredBy) : IRequest<Guid>;
