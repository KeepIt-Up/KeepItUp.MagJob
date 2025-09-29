
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response for the CreateOrganizationEndpoint.
/// </summary>
public class CreateOrganizationResponse
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
}