
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie dla endpointu CreateOrganizationEndpoint.
/// </summary>
public class CreateOrganizationRequest
{
    public const string Route = "/Organizations";

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis organizacji.
    /// </summary>
    public string? Description { get; set; }
}