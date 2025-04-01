namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Odpowiedź dla endpointu UpdateOrganizationEndpoint.
/// </summary>
public class UpdateOrganizationResponse
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis organizacji.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; set; }
}
