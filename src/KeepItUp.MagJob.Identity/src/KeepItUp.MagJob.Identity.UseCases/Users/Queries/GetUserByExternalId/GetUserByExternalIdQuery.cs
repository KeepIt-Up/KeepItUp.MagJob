using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

/// <summary>
/// Query to get a user by their external identifier.
/// </summary>
public record GetUserByExternalIdQuery : IRequest<Result<UserDto>>
{
    /// <summary>
    /// External user identifier.
    /// </summary>
    public Guid ExternalId { get; init; }
}
