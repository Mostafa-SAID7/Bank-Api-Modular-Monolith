namespace Bank.Statements.Presentation.Requests;

public sealed record CreateScheduleRequest(
    Guid CustomerId,
    Guid AccountId,
    string Period,
    string Format,
    string DeliveryMethod);
