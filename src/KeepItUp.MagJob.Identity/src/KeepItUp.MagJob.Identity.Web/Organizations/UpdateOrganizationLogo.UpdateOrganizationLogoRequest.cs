namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie dla endpointu UpdateOrganizationLogoEndpoint.
/// </summary>
public class UpdateOrganizationLogoRequest
{
    /// <summary>
    /// Ścieżka endpointu
    /// </summary>
    public static string Route => "/Organizations/{OrganizationId:guid}/Logo";

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Plik logo organizacji.
    /// </summary>
    public IFormFile? LogoFile { get; set; }
}
