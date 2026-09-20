namespace Bank.Loans.Presentation.Responses;

/// <summary>
/// Response containing EMI calculation details
/// </summary>
public record EMIResponse(
    decimal MonthlyEMI,
    decimal TotalInterest,
    decimal TotalAmountPayable,
    List<EMIScheduleItemResponse> Schedule);

/// <summary>
/// Single EMI schedule item
/// </summary>
public record EMIScheduleItemResponse(
    int InstallmentNumber,
    DateTime DueDateUtc,
    decimal PrincipalAmount,
    decimal InterestAmount,
    decimal TotalAmount,
    decimal OutstandingPrincipal);
