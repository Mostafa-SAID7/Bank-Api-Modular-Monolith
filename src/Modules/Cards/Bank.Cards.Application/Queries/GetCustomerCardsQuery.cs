namespace Bank.Cards.Application.Queries;

/// <summary>
/// Query to retrieve all cards for a customer
/// </summary>
public sealed record GetCustomerCardsQuery(Guid CustomerId) : IRequest<IEnumerable<CardDto>>;
