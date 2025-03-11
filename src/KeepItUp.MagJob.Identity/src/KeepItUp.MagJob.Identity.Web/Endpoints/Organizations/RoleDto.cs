namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// DTO dla roli w organizacji.
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis roli.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kolor roli.
    /// </summary>
    public string? Color { get; set; }
} 
