namespace Bank.Loans.Presentation.Responses;

/// <summary>
/// Response after successfully creating a loan application
/// </summary>
public record CreateLoanResponse(
    Guid LoanId,
    string LoanNumber,
    decimal ApprovedAmount,
    decimal MonthlyEMI,
    decimal TotalInterest,
    decimal TotalAmountPayable,
    string Message);
