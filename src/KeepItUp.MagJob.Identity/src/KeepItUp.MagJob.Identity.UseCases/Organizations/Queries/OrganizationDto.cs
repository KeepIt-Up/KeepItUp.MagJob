namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

/// <summary>
/// Data Transfer Object for an organization.
/// </summary>
public class OrganizationDto
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Organization description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// URL of the organization logo.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// URL of the organization banner.
    /// </summary>
    public string? BannerUrl { get; set; }

    /// <summary>
    /// Owner identifier of the organization.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Whether the organization is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// List of user roles in the organization.
    /// </summary>
    public List<string> UserRoles { get; set; } = new List<string>();
}
