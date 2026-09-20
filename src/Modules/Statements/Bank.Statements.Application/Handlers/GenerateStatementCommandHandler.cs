namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;
using Bank.Statements.Domain.Enums;

public sealed class GenerateStatementCommandHandler
{
    private readonly IStatementRepository _statementRepository;
    private readonly IStatementLineRepository _lineRepository;
    private readonly IStatementGenerator _generator;

    public GenerateStatementCommandHandler(
        IStatementRepository statementRepository,
        IStatementLineRepository lineRepository,
        IStatementGenerator generator)
    {
        _statementRepository = statementRepository;
        _lineRepository = lineRepository;
        _generator = generator;
    }

    public async Task<Guid> Handle(GenerateStatementCommand command, CancellationToken ct)
    {
        var statement = Statement.Create(
            command.CustomerId,
            command.AccountId,
            command.PeriodStartDate,
            command.PeriodEndDate,
            command.OpeningBalance,
            StatementPeriod.Monthly,
            StatementFormat.PDF);

        var lines = await _generator.GenerateLinesAsync(
            command.AccountId,
            command.PeriodStartDate,
            command.PeriodEndDate,
            command.OpeningBalance,
            ct);

        var (closingBalance, totalDebits, totalCredits) = await _generator.CalculateBalancesAsync(
            lines,
            command.OpeningBalance,
            ct);

        await _lineRepository.AddManyAsync(lines, ct);
        statement.Generate(lines.ToList(), totalDebits, totalCredits);
        await _statementRepository.AddAsync(statement, ct);

        return statement.Id;
    }
}
