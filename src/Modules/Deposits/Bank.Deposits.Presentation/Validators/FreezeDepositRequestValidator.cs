using Bank.Deposits.Presentation.Dtos;

namespace Bank.Deposits.Presentation.Validators;

/// <summary>
/// Validator for FreezeDepositRequest
/// </summary>
public sealed class FreezeDepositRequestValidator : AbstractValidator<FreezeDepositRequest>
{
    public FreezeDepositRequestValidator()
    {
        RuleFor(x => x.DepositId)
            .NotEmpty()
            .WithMessage("Deposit ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason must not exceed 500 characters");
    }
}
