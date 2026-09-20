using Bank.Deposits.Presentation.Dtos;

namespace Bank.Deposits.Presentation.Validators;

/// <summary>
/// Validator for CreateDepositRequest
/// </summary>
public sealed class CreateDepositRequestValidator : AbstractValidator<CreateDepositRequest>
{
    public CreateDepositRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.DepositTypeId)
            .NotEmpty()
            .WithMessage("Deposit type ID is required");

        RuleFor(x => x.InitialDeposit)
            .GreaterThan(0)
            .WithMessage("Initial deposit must be greater than zero");

        RuleFor(x => x.TermMonths)
            .GreaterThan(0)
            .When(x => x.IsFixedDeposit)
            .WithMessage("Term months is required for fixed deposits and must be greater than zero");
    }
}
