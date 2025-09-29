
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request for the CreateOrganizationEndpoint.
/// </summary>
public class CreateOrganizationRequest
{
    public const string Route = "/Organizations";

    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Organization description.
    /// </summary>
    public string? Description { get; set; }
}