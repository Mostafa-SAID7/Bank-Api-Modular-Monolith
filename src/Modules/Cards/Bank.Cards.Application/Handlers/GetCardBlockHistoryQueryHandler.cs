namespace Bank.Cards.Application.Handlers;

/// <summary>
/// Handler for GetCardBlockHistoryQuery
/// </summary>
public class GetCardBlockHistoryQueryHandler : IRequestHandler<GetCardBlockHistoryQuery, IEnumerable<CardBlockDto>>
{
    private readonly ICardBlockRepository _blockRepository;

    public GetCardBlockHistoryQueryHandler(ICardBlockRepository blockRepository)
    {
        _blockRepository = blockRepository;
    }

    public async Task<IEnumerable<CardBlockDto>> Handle(GetCardBlockHistoryQuery request, CancellationToken cancellationToken)
    {
        var blocks = await _blockRepository.GetByCardIdAsync(request.CardId, cancellationToken);
        return blocks.Select(MapToDto).ToList();
    }

    private static CardBlockDto MapToDto(CardBlock block)
    {
        return new CardBlockDto(
            block.Id,
            block.CardId,
            block.Reason,
            block.Description,
            block.BlockedAtUtc,
            block.UnblockedAtUtc);
    }
}
