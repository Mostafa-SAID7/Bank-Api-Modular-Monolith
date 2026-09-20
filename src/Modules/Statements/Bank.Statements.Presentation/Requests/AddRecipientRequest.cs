namespace Bank.Statements.Presentation.Requests;

public sealed record AddRecipientRequest(
    Guid StatementId,
    string Email,
    string DeliveryMethod);
