namespace Bank.Loans.Application.DTOs;

public record LoanDetailDto(
    Guid Id,
    string LoanNumber,
    Guid CustomerId,
    Guid LoanProductId,
    decimal LoanAmount,
    decimal EMIAmount,
    int TenureMonths,
    decimal AnnualInterestRate,
    int Status,
    int LoanType,
    DateTime ApplicationDateUtc,
    DateTime? ApprovalDateUtc,
    DateTime? DisbursementDateUtc,
    DateTime? ClosureDateUtc,
    decimal OutstandingPrincipal,
    decimal OutstandingInterest,
    decimal TotalPrincipalRepaid,
    decimal TotalInterestRepaid);
