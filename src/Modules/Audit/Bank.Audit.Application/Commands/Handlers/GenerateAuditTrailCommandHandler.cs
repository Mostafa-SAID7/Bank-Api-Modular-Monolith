namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;

internal sealed class GenerateAuditTrailCommandHandler : IRequestHandler<GenerateAuditTrailCommand, Guid>
{
    private readonly IAuditTrailRepository _auditTrailRepository;

    public GenerateAuditTrailCommandHandler(IAuditTrailRepository auditTrailRepository)
    {
        _auditTrailRepository = auditTrailRepository;
    }

    public async Task<Guid> Handle(GenerateAuditTrailCommand request, CancellationToken cancellationToken)
    {
        var auditTrail = AuditTrail.Create(
            request.TrailName,
            request.StartDate,
            request.EndDate,
            request.TotalEntries,
            request.FilePath,
            request.FileFormat);

        await _auditTrailRepository.AddAsync(auditTrail, cancellationToken);
        return auditTrail.Id;
    }
}
