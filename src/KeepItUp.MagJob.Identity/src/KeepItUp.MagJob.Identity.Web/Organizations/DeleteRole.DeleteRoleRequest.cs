namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to delete a role from an organization.
/// </summary>
public class DeleteRoleRequest
{
    /// <summary>
    /// Template for the URL route of the delete role endpoint.
    /// </summary>
    public const string Route = "/Organizations/{OrganizationId:guid}/Roles/{RoleId:guid}";

    /// <summary>
    /// Builds the URL route for a specific organization and role identifier.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="roleId">Role identifier to delete.</param>
    /// <returns>URL route with the specified identifiers.</returns>
    public static string BuildRoute(Guid organizationId, Guid roleId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{RoleId:guid}", roleId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Role identifier.
    /// </summary>
    public Guid RoleId { get; set; }
}
