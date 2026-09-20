namespace Bank.Statements.Presentation.Validators;

public sealed class AddRecipientValidator : AbstractValidator<AddRecipientRequest>
{
    public AddRecipientValidator()
    {
        RuleFor(x => x.StatementId)
            .NotEmpty().WithMessage("Statement ID is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.DeliveryMethod)
            .NotEmpty().WithMessage("Delivery method is required")
            .Must(d => d == "EMAIL" || d == "SMS" || d == "PORTAL")
            .WithMessage("Delivery method must be EMAIL, SMS, or PORTAL");
    }
}
