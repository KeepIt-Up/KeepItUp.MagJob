namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie dla endpointu DeleteOrganizationEndpoint.
/// </summary>
public class DeleteOrganizationRequest
{
    public const string Route = "/Organizations/{Id:guid}";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }
}
