namespace Bank.Audit.Presentation.Validators;

using Bank.Audit.Presentation.Requests;

public sealed class GenerateAuditTrailRequestValidator : AbstractValidator<GenerateAuditTrailRequest>
{
    public GenerateAuditTrailRequestValidator()
    {
        RuleFor(x => x.TrailName)
            .NotEmpty()
            .WithMessage("TrailName is required");

        RuleFor(x => x.FilePath)
            .NotEmpty()
            .WithMessage("FilePath is required");

        RuleFor(x => x.FileFormat)
            .NotEmpty()
            .WithMessage("FileFormat is required");

        RuleFor(x => x.TotalEntries)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalEntries must be non-negative");
    }
}
