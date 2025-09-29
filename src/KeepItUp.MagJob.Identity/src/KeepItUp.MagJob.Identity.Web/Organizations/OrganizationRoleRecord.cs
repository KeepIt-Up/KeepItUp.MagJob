namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// DTO for a role in an organization.
/// </summary>
public class OrganizationRoleRecord
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
    /// Role color.
    /// </summary>
    public string? Color { get; set; }
}
