namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to create a new fixed deposit account
/// </summary>
public sealed record CreateFixedDepositCommand(
    Guid CustomerId,
    Guid DepositProductId,
    decimal PrincipalAmount,
    int TermMonths,
    DateTime StartDate,
    bool EnableAutoRenewal,
    InterestCalculationMethod? InterestCalculationMethod = null) : IRequest<FixedDeposit>;
