namespace Bank.Loans.Application.DTOs;

public record LoanScheduleDto(
    Guid Id,
    int InstallmentNumber,
    DateTime DueDateUtc,
    decimal PrincipalAmount,
    decimal InterestAmount,
    decimal TotalAmount,
    decimal OutstandingPrincipal,
    bool IsPaid,
    DateTime? PaidDateUtc,
    bool IsOverdue,
    int? OverdueDays);
