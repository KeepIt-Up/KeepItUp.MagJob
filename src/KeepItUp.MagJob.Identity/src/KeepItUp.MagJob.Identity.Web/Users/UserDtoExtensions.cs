using KeepItUp.MagJob.Identity.UseCases.Users.Queries;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Extension methods for the UserDto type.
/// </summary>
public static class UserDtoExtensions
{
    /// <summary>
    /// Gets the URL of the user's profile picture.
    /// </summary>
    /// <param name="userDto">User DTO object.</param>
    /// <returns>URL of the user's profile picture or null if it does not exist.</returns>
    public static string? ProfileImageUrl(this UserDto userDto)
    {
        return userDto.Profile?.ProfileImageUrl;
    }

    /// <summary>
    /// Gets the user's phone number.
    /// </summary>
    /// <param name="userDto">User DTO object.</param>
    /// <returns>User's phone number or null if it does not exist.</returns>
    public static string? PhoneNumber(this UserDto userDto)
    {
        return userDto.Profile?.PhoneNumber;
    }

    /// <summary>
    /// Gets the user's address.
    /// </summary>
    /// <param name="userDto">User DTO object.</param>
    /// <returns>User's address or null if it does not exist.</returns>
    public static string? Address(this UserDto userDto)
    {
        return userDto.Profile?.Address;
    }
}
