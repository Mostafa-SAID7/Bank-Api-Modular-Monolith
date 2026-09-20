namespace Bank.Audit.Presentation.Validators;

using Bank.Audit.Presentation.Requests;

public sealed class CreateAuditLogRequestValidator : AbstractValidator<CreateAuditLogRequest>
{
    public CreateAuditLogRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.EventType)
            .NotEmpty()
            .WithMessage("EventType is required");

        RuleFor(x => x.ResourceType)
            .NotEmpty()
            .WithMessage("ResourceType is required");

        RuleFor(x => x.ResourceId)
            .NotEmpty()
            .WithMessage("ResourceId is required");

        RuleFor(x => x.Action)
            .NotEmpty()
            .WithMessage("Action is required");

        RuleFor(x => x.IpAddress)
            .NotEmpty()
            .WithMessage("IpAddress is required");

        RuleFor(x => x.UserAgent)
            .NotEmpty()
            .WithMessage("UserAgent is required");
    }
}
