namespace Bank.Audit.Application.Commands;

public record PurgeOldAuditsCommand(int RetentionDays) : IRequest<int>;
