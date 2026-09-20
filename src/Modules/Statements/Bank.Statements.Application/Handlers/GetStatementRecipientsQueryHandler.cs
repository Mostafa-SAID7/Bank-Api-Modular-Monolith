namespace Bank.Statements.Application.Handlers;

using Bank.Statements.Application.DTOs;
using Bank.Statements.Application.Interfaces;
using Bank.Statements.Application.Queries;

public sealed class GetStatementRecipientsQueryHandler
{
    private readonly IStatementRecipientRepository _repository;

    public GetStatementRecipientsQueryHandler(IStatementRecipientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<StatementRecipientDto>> Handle(GetStatementRecipientsQuery query, CancellationToken ct)
    {
        var recipients = await _repository.GetByStatementIdAsync(query.StatementId, ct);
        return recipients.Select(MapToDto).ToList();
    }

    private static StatementRecipientDto MapToDto(Bank.Statements.Domain.Entities.StatementRecipient recipient)
    {
        return new StatementRecipientDto(
            recipient.Id,
            recipient.Email,
            recipient.DeliveryMethod,
            recipient.HasReceived,
            recipient.ReceivedAt,
            recipient.DeliveryReference);
    }
}
