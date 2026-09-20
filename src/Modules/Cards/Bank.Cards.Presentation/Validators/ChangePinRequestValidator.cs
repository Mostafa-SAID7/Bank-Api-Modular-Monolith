namespace Bank.Cards.Presentation.Validators;

using Bank.Cards.Presentation.Dtos;

/// <summary>
/// Validator for ChangePinRequest
/// </summary>
public sealed class ChangePinRequestValidator : AbstractValidator<ChangePinRequest>
{
    public ChangePinRequestValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty()
            .WithMessage("Card ID is required");

        RuleFor(x => x.CurrentPin)
            .NotEmpty()
            .WithMessage("Current PIN is required")
            .Length(4, 6)
            .WithMessage("PIN must be between 4 and 6 digits")
            .Matches(@"^\d+$")
            .WithMessage("PIN must contain only digits");

        RuleFor(x => x.NewPin)
            .NotEmpty()
            .WithMessage("New PIN is required")
            .Length(4, 6)
            .WithMessage("PIN must be between 4 and 6 digits")
            .Matches(@"^\d+$")
            .WithMessage("PIN must contain only digits")
            .NotEqual(x => x.CurrentPin)
            .WithMessage("New PIN must be different from current PIN");
    }
}
