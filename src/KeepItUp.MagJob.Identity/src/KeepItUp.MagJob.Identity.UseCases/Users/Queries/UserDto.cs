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
    public Guid ExternalId { get; set; }

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

    /// <summary>
    /// Lista członkostw użytkownika w organizacjach.
    /// </summary>
    public List<MembershipDto> Memberships { get; set; } = new();
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

/// <summary>
/// Data Transfer Object dla członkostwa użytkownika w organizacji.
/// </summary>
public class MembershipDto
{
    /// <summary>
    /// Identyfikator członkostwa.
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Data dołączenia do organizacji.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Lista identyfikatorów ról przypisanych do członka.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
