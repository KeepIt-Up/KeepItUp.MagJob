using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;

/// <summary>
/// Command to deactivate a user.
/// </summary>
public record DeactivateUserCommand : IRequest<Result>
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; init; }
}
