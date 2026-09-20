namespace Bank.Cards.Application.Commands;

/// <summary>
/// Command to issue a new card to a customer
/// </summary>
public sealed record IssueCardCommand(
    Guid CustomerId,
    Guid LinkedAccountId,
    Guid CardProductId,
    string HolderName) : IRequest<Card>;
