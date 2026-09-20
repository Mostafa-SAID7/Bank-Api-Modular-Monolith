namespace Bank.Audit.Application.Commands;

using Bank.Audit.Domain.Enums;

public record UpdateAuditConfigurationCommand(
    Guid ConfigurationId,
    AuditLevel AuditLevel,
    int RetentionDays,
    bool EnableEmailNotifications,
    string NotificationEmail,
    string ModifiedBy) : IRequest<bool>;
