namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response for the UpdateOrganizationLogoEndpoint.
/// </summary>
public class UpdateOrganizationLogoResponse
{
    /// <summary>
    /// Organization logo URL.
    /// </summary>
    public string LogoUrl { get; set; } = string.Empty;
}
