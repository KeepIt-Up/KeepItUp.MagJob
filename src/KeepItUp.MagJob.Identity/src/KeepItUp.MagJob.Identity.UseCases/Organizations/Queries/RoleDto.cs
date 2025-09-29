namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

/// <summary>
/// Data Transfer Object for a role in an organization.
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Role identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Role name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Role color (in HEX format).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// List of permissions assigned to the role.
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();
}
