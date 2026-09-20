namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Repositories;

internal sealed class ExportAuditLogCommandHandler : IRequestHandler<ExportAuditLogCommand, string>
{
    private readonly IAuditExportService _auditExportService;

    public ExportAuditLogCommandHandler(IAuditExportService auditExportService)
    {
        _auditExportService = auditExportService;
    }

    public async Task<string> Handle(ExportAuditLogCommand request, CancellationToken cancellationToken)
    {
        var exportPath = await _auditExportService.ExportAuditLogAsync(request.AuditLogId, request.ExportFormat, cancellationToken);
        return exportPath;
    }
}
