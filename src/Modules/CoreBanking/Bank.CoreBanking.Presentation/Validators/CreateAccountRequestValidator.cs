using FluentValidation;
using Bank.CoreBanking.Presentation.Dtos;

namespace Bank.CoreBanking.Presentation.Validators;

/// <summary>
/// Validator for CreateAccountRequest
/// Ensures all required fields are present and valid format
/// </summary>
public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required and must not be empty.");

        RuleFor(x => x.AccountHolderName)
            .NotEmpty()
            .WithMessage("AccountHolderName is required.")
            .MaximumLength(100)
            .WithMessage("AccountHolderName cannot exceed 100 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency must be a valid 3-letter ISO 4217 code.")
            .Must(c => c.All(char.IsLetter) && c == c.ToUpperInvariant())
            .WithMessage("Currency must be 3 uppercase letters (ISO 4217 format).");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid account type.");
    }
}
