namespace Bank.Loans.Presentation.Requests;

/// <summary>
/// Request to record an EMI payment
/// </summary>
public record RecordPaymentRequest(
    Guid LoanId,
    decimal Amount,
    Guid ScheduleId);
