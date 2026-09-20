namespace Bank.Audit.Application.Queries;

using Bank.Audit.Application.DTOs;

public record GetAuditsByUserQuery(string UserId) : IRequest<IEnumerable<AuditLogDto>>;
