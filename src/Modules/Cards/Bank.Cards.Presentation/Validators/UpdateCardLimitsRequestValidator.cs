namespace Bank.Cards.Presentation.Validators;

using Bank.Cards.Presentation.Dtos;

/// <summary>
/// Validator for UpdateCardLimitsRequest
/// </summary>
public sealed class UpdateCardLimitsRequestValidator : AbstractValidator<UpdateCardLimitsRequest>
{
    public UpdateCardLimitsRequestValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty()
            .WithMessage("Card ID is required");

        RuleFor(x => x.DailyWithdrawalLimit)
            .GreaterThan(0)
            .WithMessage("Daily withdrawal limit must be greater than zero")
            .LessThanOrEqualTo(100000)
            .WithMessage("Daily withdrawal limit cannot exceed 100,000");

        RuleFor(x => x.DailyTransactionLimit)
            .GreaterThan(0)
            .WithMessage("Daily transaction limit must be greater than zero")
            .LessThanOrEqualTo(500000)
            .WithMessage("Daily transaction limit cannot exceed 500,000");

        RuleFor(x => x.DailyForeignTransactionLimit)
            .GreaterThan(0)
            .WithMessage("Daily foreign transaction limit must be greater than zero")
            .LessThanOrEqualTo(500000)
            .WithMessage("Daily foreign transaction limit cannot exceed 500,000");
    }
}
