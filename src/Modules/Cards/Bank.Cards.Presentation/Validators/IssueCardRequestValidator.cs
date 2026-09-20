namespace Bank.Cards.Presentation.Validators;

using Bank.Cards.Presentation.Dtos;

/// <summary>
/// Validator for IssueCardRequest
/// </summary>
public sealed class IssueCardRequestValidator : AbstractValidator<IssueCardRequest>
{
    public IssueCardRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.LinkedAccountId)
            .NotEmpty()
            .WithMessage("Linked account ID is required");

        RuleFor(x => x.CardProductId)
            .NotEmpty()
            .WithMessage("Card product ID is required");

        RuleFor(x => x.HolderName)
            .NotEmpty()
            .WithMessage("Cardholder name is required")
            .Length(2, 100)
            .WithMessage("Cardholder name must be between 2 and 100 characters");
    }
}
