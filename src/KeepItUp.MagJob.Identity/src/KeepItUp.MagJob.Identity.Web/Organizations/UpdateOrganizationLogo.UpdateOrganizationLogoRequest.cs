namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request for the UpdateOrganizationLogoEndpoint.
/// </summary>
public class UpdateOrganizationLogoRequest
{
    /// <summary>
    /// Endpoint route.
    /// </summary>
    public static string Route => "/Organizations/{OrganizationId:guid}/Logo";

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Organization logo file.
    /// </summary>
    public IFormFile? LogoFile { get; set; }
}
