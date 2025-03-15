namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

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
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Adres email zapraszanego użytkownika.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Token zaproszenia.
    /// </summary>
    public string Token { get; set; } = string.Empty;

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
    /// Rola przypisana do zaproszenia.
    /// </summary>
    public RoleDto? Role { get; set; }

    /// <summary>
    /// Data utworzenia zaproszenia.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Identyfikator użytkownika, który utworzył zaproszenie.
    /// </summary>
    public Guid CreatedBy { get; set; }
} 
