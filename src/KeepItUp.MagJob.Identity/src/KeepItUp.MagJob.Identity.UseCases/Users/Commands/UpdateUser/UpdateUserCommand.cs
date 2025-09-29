using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

/// <summary>
/// Command to update an existing user.
/// </summary>
public record UpdateUserCommand : IRequest<Result<EmptyResponse>>
{
    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User last name.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// User phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// User address (optional).
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// URL of the user's profile picture (optional).
    /// </summary>
    public string? ProfileImageUrl { get; init; }
}
