namespace Bank.Cards.Application.Queries;

/// <summary>
/// Query to retrieve all available card products
/// </summary>
public sealed record GetCardProductsQuery : IRequest<IEnumerable<CardProductDto>>;
