namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie dla endpointu UpdateOrganizationBannerEndpoint.
/// </summary>
public class UpdateOrganizationBannerRequest
{
    /// <summary>
    /// Ścieżka endpointu
    /// </summary>
    public static string Route => "/Organizations/{OrganizationId:guid}/Banner";

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Plik bannera organizacji.
    /// </summary>
    public IFormFile? BannerFile { get; set; }
}
