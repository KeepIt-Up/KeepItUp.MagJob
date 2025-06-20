namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Request for the UpdateOrganizationEndpoint.
/// </summary>
public class UpdateOrganizationRequest
{
    public const string Route = "/Organizations/{Id:guid}";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Organization description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the organization should be active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
