namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

/// <summary>
/// Data Transfer Object dla organizacji.
/// </summary>
public class OrganizationDto
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis organizacji.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Czy organizacja jest aktywna.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Lista ról użytkownika w organizacji.
    /// </summary>
    public List<string> UserRoles { get; set; } = new List<string>();
} 
