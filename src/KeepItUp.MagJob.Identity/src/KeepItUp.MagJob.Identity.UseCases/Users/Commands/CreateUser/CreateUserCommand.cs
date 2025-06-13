using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;

/// <summary>
/// Command to create a new user.
/// </summary>
public record CreateUserCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// External user identifier in the external system (Keycloak).
    /// </summary>
    public required Guid ExternalId { get; init; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User name.
    /// </summary>
    public string Username { get; init; } = string.Empty;

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
