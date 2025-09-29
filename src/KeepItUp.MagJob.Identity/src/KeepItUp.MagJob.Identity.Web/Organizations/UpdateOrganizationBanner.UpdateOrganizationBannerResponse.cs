namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Response for the UpdateOrganizationBannerEndpoint.
/// </summary>
public class UpdateOrganizationBannerResponse
{
    /// <summary>
    /// Organization banner URL.
    /// </summary>
    public string BannerUrl { get; set; } = string.Empty;
}
