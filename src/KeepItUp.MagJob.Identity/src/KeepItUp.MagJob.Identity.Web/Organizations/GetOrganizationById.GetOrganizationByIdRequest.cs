namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie dla endpointu GetOrganizationById.
/// </summary>
public class GetOrganizationByIdRequest
{
    public const string Route = "/Organizations/{Id:guid}";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }
} 
