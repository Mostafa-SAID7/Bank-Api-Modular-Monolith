namespace Bank.Statements.Application.DTOs;

using Bank.Statements.Domain.Enums;

public sealed record StatementRecipientDto(
    Guid Id,
    string Email,
    DeliveryMethod DeliveryMethod,
    bool HasReceived,
    DateTime? ReceivedAt,
    string? DeliveryReference);
