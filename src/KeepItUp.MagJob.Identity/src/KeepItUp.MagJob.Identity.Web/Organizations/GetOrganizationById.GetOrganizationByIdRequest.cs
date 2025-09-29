namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request for the GetOrganizationByIdEndpoint.
/// </summary>
public class GetOrganizationByIdRequest
{
    public const string Route = "/Organizations/{Id:guid}";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid Id { get; set; }
}
