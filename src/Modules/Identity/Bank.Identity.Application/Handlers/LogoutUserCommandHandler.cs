using Bank.Identity.Application.Commands;

namespace Bank.Identity.Application.Handlers;

/// <summary>
/// Handler for LogoutUserCommand
/// Terminates user session
/// </summary>
public sealed class LogoutUserCommandHandler
{
    private readonly ISessionRepository _sessionRepository;

    public LogoutUserCommandHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<LogoutUserResult> Handle(
        LogoutUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var session = await _sessionRepository.GetByIdAsync(command.SessionId, cancellationToken);
            if (session == null)
                return new LogoutUserResult(command.SessionId, false, "Session not found");

            session.Terminate(command.Reason);
            await _sessionRepository.UpdateAsync(session, cancellationToken);
            await _sessionRepository.SaveChangesAsync(cancellationToken);

            return new LogoutUserResult(command.SessionId, true, "Logout successful");
        }
        catch (Exception ex)
        {
            return new LogoutUserResult(command.SessionId, false, $"Logout failed: {ex.Message}");
        }
    }
}
