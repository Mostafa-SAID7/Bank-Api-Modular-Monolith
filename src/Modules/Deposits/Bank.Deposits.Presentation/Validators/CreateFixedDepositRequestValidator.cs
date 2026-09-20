using Bank.Deposits.Presentation.Dtos;

namespace Bank.Deposits.Presentation.Validators;

/// <summary>
/// Validator for CreateFixedDepositRequest
/// </summary>
public sealed class CreateFixedDepositRequestValidator : AbstractValidator<CreateFixedDepositRequest>
{
    public CreateFixedDepositRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.DepositProductId)
            .NotEmpty()
            .WithMessage("Deposit product ID is required");

        RuleFor(x => x.PrincipalAmount)
            .GreaterThan(0)
            .WithMessage("Principal amount must be greater than zero");

        RuleFor(x => x.TermMonths)
            .GreaterThan(0)
            .WithMessage("Term months must be greater than zero");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .Must(d => d <= DateTime.UtcNow.AddDays(1))
            .WithMessage("Start date cannot be more than 1 day in the future");

        RuleFor(x => x.InterestCalculationMethod)
            .InclusiveBetween(0, 2)
            .When(x => x.InterestCalculationMethod.HasValue)
            .WithMessage("Interest calculation method is invalid");
    }
}
