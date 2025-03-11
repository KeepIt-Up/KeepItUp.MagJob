namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// DTO dla zaproszenia do organizacji.
/// </summary>
public class InvitationDto
{
    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Adres email osoby zapraszanej.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Status zaproszenia.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Data wygaśnięcia zaproszenia.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Czy zaproszenie wygasło.
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Data utworzenia zaproszenia.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Identyfikator użytkownika, który utworzył zaproszenie.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Rola, która zostanie przypisana po akceptacji zaproszenia.
    /// </summary>
    public RoleDto? Role { get; set; }
} 
