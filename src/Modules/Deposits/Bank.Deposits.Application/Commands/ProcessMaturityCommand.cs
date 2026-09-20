namespace Bank.Deposits.Application.Commands;

/// <summary>
/// Command to process maturity of a fixed deposit
/// </summary>
public sealed record ProcessMaturityCommand(
    Guid FixedDepositId,
    MaturityAction MaturityAction,
    Guid? RenewalProductId = null,
    int? RenewalTermMonths = null) : IRequest<FixedDeposit>;
