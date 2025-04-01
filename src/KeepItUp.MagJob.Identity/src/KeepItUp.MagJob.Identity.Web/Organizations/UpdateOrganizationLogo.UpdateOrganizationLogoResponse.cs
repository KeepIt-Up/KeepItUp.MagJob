namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź dla endpointu UpdateOrganizationLogoEndpoint.
/// </summary>
public class UpdateOrganizationLogoResponse
{
    /// <summary>
    /// URL do logo organizacji.
    /// </summary>
    public string LogoUrl { get; set; } = string.Empty;
}
