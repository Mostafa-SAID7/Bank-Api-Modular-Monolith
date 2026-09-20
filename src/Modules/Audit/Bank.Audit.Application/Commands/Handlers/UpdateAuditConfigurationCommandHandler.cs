namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Repositories;

internal sealed class UpdateAuditConfigurationCommandHandler : IRequestHandler<UpdateAuditConfigurationCommand, bool>
{
    private readonly IAuditConfigurationRepository _configurationRepository;

    public UpdateAuditConfigurationCommandHandler(IAuditConfigurationRepository configurationRepository)
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<bool> Handle(UpdateAuditConfigurationCommand request, CancellationToken cancellationToken)
    {
        var configuration = await _configurationRepository.GetByIdAsync(request.ConfigurationId, cancellationToken);
        if (configuration is null)
            return false;

        configuration.UpdateLevel(request.AuditLevel, request.ModifiedBy);
        configuration.UpdateRetentionPolicy(request.RetentionDays, request.ModifiedBy);

        await _configurationRepository.UpdateAsync(configuration, cancellationToken);
        return true;
    }
}
