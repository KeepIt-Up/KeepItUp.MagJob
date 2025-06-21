namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź dla endpointu UpdateOrganizationBannerEndpoint.
/// </summary>
public class UpdateOrganizationBannerResponse
{
    /// <summary>
    /// URL do bannera organizacji.
    /// </summary>
    public string BannerUrl { get; set; } = string.Empty;
}
