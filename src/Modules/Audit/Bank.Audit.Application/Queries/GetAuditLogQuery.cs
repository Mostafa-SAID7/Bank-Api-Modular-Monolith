namespace Bank.Audit.Application.Queries;

using Bank.Audit.Application.DTOs;

public record GetAuditLogQuery(Guid AuditLogId) : IRequest<AuditLogDto?>;
