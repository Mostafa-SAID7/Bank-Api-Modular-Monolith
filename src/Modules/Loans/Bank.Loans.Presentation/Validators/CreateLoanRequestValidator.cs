namespace Bank.Loans.Presentation.Validators;

/// <summary>
/// Validator for CreateLoanRequest
/// </summary>
public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.LoanProductId)
            .NotEmpty().WithMessage("Loan Product ID is required");

        RuleFor(x => x.LoanAmount)
            .GreaterThan(0).WithMessage("Loan amount must be greater than 0")
            .LessThanOrEqualTo(10000000).WithMessage("Loan amount cannot exceed 10,000,000");

        RuleFor(x => x.TenureMonths)
            .GreaterThan(0).WithMessage("Tenure must be greater than 0")
            .LessThanOrEqualTo(360).WithMessage("Tenure cannot exceed 360 months (30 years)");
    }
}
