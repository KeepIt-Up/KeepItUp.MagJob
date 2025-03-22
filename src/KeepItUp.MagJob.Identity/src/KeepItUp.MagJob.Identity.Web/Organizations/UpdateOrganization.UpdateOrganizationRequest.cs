namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Żądanie dla endpointu UpdateOrganizationEndpoint.
/// </summary>
public class UpdateOrganizationRequest
{
    public const string Route = "/Organizations/{Id:guid}";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

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
} 
