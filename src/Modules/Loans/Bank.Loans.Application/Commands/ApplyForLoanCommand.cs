namespace Bank.Loans.Application.Commands;

public record ApplyForLoanCommand(
    Guid CustomerId,
    Guid LoanProductId,
    decimal LoanAmount,
    int TenureMonths,
    string? CollateralDescription = null,
    decimal? CollateralValue = null) : IRequest<Loan>;
