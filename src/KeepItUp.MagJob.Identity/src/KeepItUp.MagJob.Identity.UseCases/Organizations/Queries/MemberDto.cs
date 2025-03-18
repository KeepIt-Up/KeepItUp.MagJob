namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

/// <summary>
/// DTO dla członka organizacji.
/// </summary>
public class MemberDto
{
    /// <summary>
    /// Identyfikator członka.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Adres email użytkownika.
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
    /// Nazwa wyświetlana użytkownika.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Data dołączenia do organizacji.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Role przypisane do członka.
    /// </summary>
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
}
