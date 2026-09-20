namespace Bank.Statements.Presentation.Validators;

public sealed class GenerateStatementValidator : AbstractValidator<GenerateStatementRequest>
{
    public GenerateStatementValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.PeriodStartDate)
            .NotEmpty().WithMessage("Period start date is required")
            .LessThan(x => x.PeriodEndDate).WithMessage("Period start date must be before end date");

        RuleFor(x => x.PeriodEndDate)
            .NotEmpty().WithMessage("Period end date is required")
            .GreaterThan(x => x.PeriodStartDate).WithMessage("Period end date must be after start date");

        RuleFor(x => x.OpeningBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Opening balance must be non-negative");
    }
}
