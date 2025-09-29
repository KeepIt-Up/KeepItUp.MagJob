using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Query to get a user by their identifier.
/// </summary>
public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; init; }
}
