using Bank.Deposits.Presentation.Dtos;

namespace Bank.Deposits.Presentation.Validators;

/// <summary>
/// Validator for ProcessMaturityRequest
/// </summary>
public sealed class ProcessMaturityRequestValidator : AbstractValidator<ProcessMaturityRequest>
{
    public ProcessMaturityRequestValidator()
    {
        RuleFor(x => x.FixedDepositId)
            .NotEmpty()
            .WithMessage("Fixed deposit ID is required");

        RuleFor(x => x.MaturityAction)
            .InclusiveBetween(0, 2)
            .WithMessage("Maturity action is invalid");

        RuleFor(x => x.RenewalProductId)
            .NotEmpty()
            .When(x => x.MaturityAction == 1) // Renew
            .WithMessage("Renewal product ID is required when renewing the deposit");

        RuleFor(x => x.RenewalTermMonths)
            .GreaterThan(0)
            .When(x => x.MaturityAction == 1) // Renew
            .WithMessage("Renewal term months is required and must be greater than zero when renewing");
    }
}
