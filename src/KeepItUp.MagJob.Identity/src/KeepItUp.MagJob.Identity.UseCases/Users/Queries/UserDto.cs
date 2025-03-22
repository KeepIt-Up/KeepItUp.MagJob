namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries;

/// <summary>
/// Data Transfer Object dla użytkownika.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identyfikator użytkownika w systemie zewnętrznym (Keycloak).
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Czy użytkownik jest aktywny.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Profil użytkownika.
    /// </summary>
    public UserProfileDto? Profile { get; set; }
}

/// <summary>
/// Data Transfer Object dla profilu użytkownika.
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// Numer telefonu użytkownika.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Adres użytkownika.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// URL do zdjęcia profilowego użytkownika.
    /// </summary>
    public string? ProfileImageUrl { get; set; }
} 
