using Bank.Identity.Application.Queries;

namespace Bank.Identity.Application.Handlers;

/// <summary>
/// Handler for GetUserByIdQuery
/// </summary>
public sealed class GetUserByIdQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserByIdResult?> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        
        if (user == null)
            return null;

        return new GetUserByIdResult(
            user.Id,
            user.Email ?? "",
            user.FirstName,
            user.LastName,
            user.FullName,
            user.PhoneNumber,
            (int)user.Status,
            user.CreatedAtUtc,
            user.LastLoginAtUtc,
            user.IsActive()
        );
    }
}
