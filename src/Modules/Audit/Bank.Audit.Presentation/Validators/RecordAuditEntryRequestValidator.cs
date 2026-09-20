namespace Bank.Audit.Presentation.Validators;

using Bank.Audit.Presentation.Requests;

public sealed class RecordAuditEntryRequestValidator : AbstractValidator<RecordAuditEntryRequest>
{
    public RecordAuditEntryRequestValidator()
    {
        RuleFor(x => x.AuditLogId)
            .NotEmpty()
            .WithMessage("AuditLogId is required");

        RuleFor(x => x.EntityName)
            .NotEmpty()
            .WithMessage("EntityName is required");

        RuleFor(x => x.ActionType)
            .NotEmpty()
            .WithMessage("ActionType is required");

        RuleFor(x => x.ChangeType)
            .NotEmpty()
            .WithMessage("ChangeType is required");
    }
}
