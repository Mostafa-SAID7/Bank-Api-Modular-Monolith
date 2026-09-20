namespace Bank.Audit.Application.Queries;

using Bank.Audit.Application.DTOs;

public record GetAuditsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<IEnumerable<AuditLogDto>>;
