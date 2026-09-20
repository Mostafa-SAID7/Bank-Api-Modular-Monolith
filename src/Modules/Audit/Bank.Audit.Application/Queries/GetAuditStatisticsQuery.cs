namespace Bank.Audit.Application.Queries;

using Bank.Audit.Application.DTOs;

public record GetAuditStatisticsQuery : IRequest<AuditStatisticsDto>;
