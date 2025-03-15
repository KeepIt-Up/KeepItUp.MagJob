namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

/// <summary>
/// Data Transfer Object dla roli w organizacji.
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
    /// Kolor roli (w formacie HEX).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Lista uprawnień przypisanych do roli.
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();
} 
