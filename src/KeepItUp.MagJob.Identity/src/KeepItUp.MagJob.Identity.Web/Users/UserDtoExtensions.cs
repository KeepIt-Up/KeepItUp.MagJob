using KeepItUp.MagJob.Identity.UseCases.Users.Queries;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Klasa rozszerzeń dla typu UserDto.
/// </summary>
public static class UserDtoExtensions
{
    /// <summary>
    /// Pobiera URL zdjęcia profilowego użytkownika.
    /// </summary>
    /// <param name="userDto">Obiekt DTO użytkownika.</param>
    /// <returns>URL zdjęcia profilowego lub null, jeśli nie istnieje.</returns>
    public static string? ProfileImageUrl(this UserDto userDto)
    {
        return userDto.Profile?.ProfileImageUrl;
    }

    /// <summary>
    /// Pobiera numer telefonu użytkownika.
    /// </summary>
    /// <param name="userDto">Obiekt DTO użytkownika.</param>
    /// <returns>Numer telefonu lub null, jeśli nie istnieje.</returns>
    public static string? PhoneNumber(this UserDto userDto)
    {
        return userDto.Profile?.PhoneNumber;
    }

    /// <summary>
    /// Pobiera adres użytkownika.
    /// </summary>
    /// <param name="userDto">Obiekt DTO użytkownika.</param>
    /// <returns>Adres użytkownika lub null, jeśli nie istnieje.</returns>
    public static string? Address(this UserDto userDto)
    {
        return userDto.Profile?.Address;
    }
}
