namespace Bank.Cards.Presentation.Validators;

using Bank.Cards.Presentation.Dtos;

/// <summary>
/// Validator for BlockCardRequest
/// </summary>
public sealed class BlockCardRequestValidator : AbstractValidator<BlockCardRequest>
{
    public BlockCardRequestValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty()
            .WithMessage("Card ID is required");

        RuleFor(x => x.Reason)
            .IsInEnum()
            .WithMessage("Block reason must be a valid enum value");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
