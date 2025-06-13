namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request for the UpdateOrganizationBannerEndpoint.
/// </summary>
public class UpdateOrganizationBannerRequest
{
    /// <summary>
    /// Endpoint route.
    /// </summary>
    public static string Route => "/Organizations/{OrganizationId:guid}/Banner";

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Organization banner file.
    /// </summary>
    public IFormFile? BannerFile { get; set; }
}
