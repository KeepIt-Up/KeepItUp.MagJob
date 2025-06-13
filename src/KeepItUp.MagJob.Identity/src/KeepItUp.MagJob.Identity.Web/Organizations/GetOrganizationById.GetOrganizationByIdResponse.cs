namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response for the GetOrganizationByIdEndpoint.
/// </summary>
public class GetOrganizationByIdResponse
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
    /// Owner identifier.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Whether the user is the owner of the organization.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Number of organization members.
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// Organization logo URL.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Organization banner URL.
    /// </summary>
    public string? BannerUrl { get; set; }

}
