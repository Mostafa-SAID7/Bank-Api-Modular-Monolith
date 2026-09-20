namespace Bank.Statements.Application.Commands;

using Bank.Statements.Domain.Enums;

public sealed record AddRecipientCommand(
    Guid StatementId,
    string Email,
    DeliveryMethod DeliveryMethod);
