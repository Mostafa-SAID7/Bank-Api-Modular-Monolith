using Bank.Deposits.Presentation.Dtos;

namespace Bank.Deposits.Presentation.Validators;

/// <summary>
/// Validator for DepositTransactionRequest (deposit/withdrawal)
/// </summary>
public sealed class DepositTransactionRequestValidator : AbstractValidator<DepositTransactionRequest>
{
    public DepositTransactionRequestValidator()
    {
        RuleFor(x => x.DepositId)
            .NotEmpty()
            .WithMessage("Deposit ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");
    }
}
