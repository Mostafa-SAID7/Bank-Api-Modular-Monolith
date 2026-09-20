namespace Bank.Deposits.Application.DTOs;

/// <summary>
/// DTO for maturity notice details
/// </summary>
public record MaturityNoticeDto(
    Guid Id,
    Guid FixedDepositId,
    string FixedDepositNumber,
    Guid CustomerId,
    DateTime MaturityDate,
    DateTime DueDateUtc,
    bool NotificationSent,
    DateTime? NotificationSentAtUtc,
    bool CustomerChoiceProvided,
    DateTime? CustomerChoiceProvidedAtUtc,
    string? CustomerChoice,
    DateTime CreatedAtUtc,
    DateTime LastModifiedAtUtc);
