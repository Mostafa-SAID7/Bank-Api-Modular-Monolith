namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to enable or disable auto-renewal for a fixed deposit
/// </summary>
public sealed record EnableAutoRenewalCommand(
    Guid FixedDepositId,
    bool EnableAutoRenewal) : IRequest<FixedDeposit>;
