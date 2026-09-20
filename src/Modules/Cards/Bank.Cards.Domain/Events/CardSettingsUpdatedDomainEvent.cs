namespace Bank.Cards.Domain.Events;

/// <summary>
/// Domain event raised when card settings are updated (international, contactless, online)
/// </summary>
public sealed record CardSettingsUpdatedDomainEvent(
    Guid CardId,
    Guid CustomerId,
    string SettingName,
    object NewValue) : DomainEvent(CardId, "CardSettingsUpdated");
