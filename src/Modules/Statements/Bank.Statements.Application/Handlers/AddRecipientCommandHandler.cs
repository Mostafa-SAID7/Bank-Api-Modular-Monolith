namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.Commands;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Domain.Entities;

public sealed class AddRecipientCommandHandler
{
    private readonly IStatementRepository _statementRepository;
    private readonly IStatementRecipientRepository _recipientRepository;

    public AddRecipientCommandHandler(
        IStatementRepository statementRepository,
        IStatementRecipientRepository recipientRepository)
    {
        _statementRepository = statementRepository;
        _recipientRepository = recipientRepository;
    }

    public async Task<Guid> Handle(AddRecipientCommand command, CancellationToken ct)
    {
        var statement = await _statementRepository.GetByIdAsync(command.StatementId, ct);
        if (statement == null)
            throw new InvalidOperationException($"Statement {command.StatementId} not found");

        var recipient = StatementRecipient.Create(
            command.StatementId,
            command.Email,
            command.DeliveryMethod);

        statement.AddRecipient(recipient);
        await _recipientRepository.AddAsync(recipient, ct);

        return recipient.Id;
    }
}
