namespace Bank.Statements.Presentation.Validators;

public sealed class CreateScheduleValidator : AbstractValidator<CreateScheduleRequest>
{
    public CreateScheduleValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Period)
            .NotEmpty().WithMessage("Period is required")
            .Must(p => p == "MONTHLY" || p == "QUARTERLY" || p == "ANNUAL" || p == "WEEKLY" || p == "DAILY")
            .WithMessage("Period must be DAILY, WEEKLY, MONTHLY, QUARTERLY, or ANNUAL");

        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required")
            .Must(f => f == "PDF" || f == "CSV" || f == "JSON")
            .WithMessage("Format must be PDF, CSV, or JSON");

        RuleFor(x => x.DeliveryMethod)
            .NotEmpty().WithMessage("Delivery method is required")
            .Must(d => d == "EMAIL" || d == "SMS" || d == "PORTAL")
            .WithMessage("Delivery method must be EMAIL, SMS, or PORTAL");
    }
}
