using MediatR;
using Microsoft.AspNetCore.Http;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUserProfilePicture;

/// <summary>
/// Command to update the profile picture of a user.
/// </summary>
public record UpdateUserProfilePictureCommand : IRequest<Result<string>>
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Profile picture file.
    /// </summary>
    public IFormFile ProfilePictureFile { get; init; } = null!;

    /// <summary>
    /// Current user identifier performing the operation.
    /// </summary>
    public Guid CurrentUserId { get; init; }
}