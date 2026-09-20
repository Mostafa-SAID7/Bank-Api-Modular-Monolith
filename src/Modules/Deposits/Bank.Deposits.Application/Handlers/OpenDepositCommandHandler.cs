using Bank.Deposits.Application.Commands;
using Bank.Deposits.Application.Interfaces;

namespace Bank.Deposits.Application.Handlers;

/// <summary>
/// Handler for OpenDepositCommand
/// </summary>
public class OpenDepositCommandHandler : ICommandHandler<OpenDepositCommand>
{
    private readonly IDepositRepository _depositRepository;
    private readonly IDepositTypeRepository _depositTypeRepository;
    private readonly IInterestRateRepository _interestRateRepository;
    private readonly IAccountNumberGenerator _accountNumberGenerator;

    public OpenDepositCommandHandler(
        IDepositRepository depositRepository,
        IDepositTypeRepository depositTypeRepository,
        IInterestRateRepository interestRateRepository,
        IAccountNumberGenerator accountNumberGenerator)
    {
        _depositRepository = depositRepository;
        _depositTypeRepository = depositTypeRepository;
        _interestRateRepository = interestRateRepository;
        _accountNumberGenerator = accountNumberGenerator;
    }

    public async Task<Result> Handle(OpenDepositCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve and validate deposit type
            var depositType = await _depositTypeRepository.GetByIdAsync(command.DepositTypeId, cancellationToken);
            if (depositType is null)
                return Result.Failure(new Error("404", "Deposit type not found"));

            if (!depositType.IsActive)
                return Result.Failure(new Error("400", "Deposit type is not active"));

            // Validate initial deposit
            if (command.InitialDeposit < depositType.MinimumBalance)
                return Result.Failure(new Error("400", 
                    $"Initial deposit must be at least {depositType.MinimumBalance}"));

            if (depositType.MaximumBalance > 0 && command.InitialDeposit > depositType.MaximumBalance)
                return Result.Failure(new Error("400", 
                    $"Initial deposit exceeds maximum balance of {depositType.MaximumBalance}"));

            // Get current interest rate
            var interestRate = await _interestRateRepository.GetCurrentRateAsync(command.DepositTypeId, cancellationToken);
            if (interestRate is null)
                return Result.Failure(new Error("400", "No active interest rate for this deposit type"));

            // Generate account number
            var accountNumber = await _accountNumberGenerator.GenerateAsync(cancellationToken);

            // Create deposit aggregate
            var deposit = Deposit.Create(
                accountNumber,
                command.CustomerId,
                command.DepositTypeId,
                depositType,
                command.InitialDeposit,
                command.IsFixedDeposit,
                command.TermMonths,
                interestRate.Frequency);

            // Persist deposit
            await _depositRepository.AddAsync(deposit, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"Error opening deposit: {ex.Message}"));
        }
    }
}
