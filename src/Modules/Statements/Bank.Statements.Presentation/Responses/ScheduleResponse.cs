namespace Bank.Statements.Presentation.Responses;

public sealed record ScheduleResponse(
    Guid Id,
    Guid CustomerId,
    Guid AccountId,
    string Period,
    string Format,
    string DeliveryMethod,
    bool IsActive,
    DateTime? NextGenerationDate,
    DateTime? LastGeneratedDate,
    DateTime CreatedAt);
