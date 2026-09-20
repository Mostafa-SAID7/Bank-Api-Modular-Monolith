namespace Bank.Audit.Application.Commands.Handlers;

using Bank.Audit.Domain.Entities;
using Bank.Audit.Domain.Repositories;

internal sealed class ConfigureAuditLevelCommandHandler : IRequestHandler<ConfigureAuditLevelCommand, Guid>
{
    private readonly IAuditConfigurationRepository _configurationRepository;

    public ConfigureAuditLevelCommandHandler(IAuditConfigurationRepository configurationRepository)
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<Guid> Handle(ConfigureAuditLevelCommand request, CancellationToken cancellationToken)
    {
        var configuration = AuditConfiguration.Create(
            request.AuditLevel,
            30,
            false,
            string.Empty,
            request.ConfiguredBy);

        await _configurationRepository.AddAsync(configuration, cancellationToken);
        return configuration.Id;
    }
}
