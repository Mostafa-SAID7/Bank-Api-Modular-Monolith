namespace Bank.Cards.Presentation.Validators;

using Bank.Cards.Presentation.Dtos;

/// <summary>
/// Validator for ActivateCardRequest
/// </summary>
public sealed class ActivateCardRequestValidator : AbstractValidator<ActivateCardRequest>
{
    public ActivateCardRequestValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty()
            .WithMessage("Card ID is required");

        RuleFor(x => x.Pin)
            .NotEmpty()
            .WithMessage("PIN is required")
            .Length(4, 6)
            .WithMessage("PIN must be between 4 and 6 digits")
            .Matches(@"^\d+$")
            .WithMessage("PIN must contain only digits");
    }
}
