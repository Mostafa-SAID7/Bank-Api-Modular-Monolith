namespace Bank.Loans.Application.Commands;

public record RecordEMIPaymentCommand(
    Guid LoanId,
    decimal Amount,
    Guid ScheduleId) : IRequest<Loan>;
