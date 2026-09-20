namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Repositories;

internal sealed class PurgeOldAuditsCommandHandler : IRequestHandler<PurgeOldAuditsCommand, int>
{
    private readonly IAuditPurgeService _auditPurgeService;

    public PurgeOldAuditsCommandHandler(IAuditPurgeService auditPurgeService)
    {
        _auditPurgeService = auditPurgeService;
    }

    public async Task<int> Handle(PurgeOldAuditsCommand request, CancellationToken cancellationToken)
    {
        var purgedCount = await _auditPurgeService.PurgeOlderThanAsync(request.RetentionDays, cancellationToken);
        return purgedCount;
    }
}
