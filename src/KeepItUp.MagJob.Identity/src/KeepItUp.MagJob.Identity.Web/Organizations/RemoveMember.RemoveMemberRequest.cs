namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to remove a member from an organization.
/// </summary>
public class RemoveMemberRequest
{
    /// <summary>
    /// Template for the URL route of the endpoint to remove a member from an organization.
    /// </summary>
    public const string Route = "/Organizations/{OrganizationId:guid}/Members/{MemberUserId:guid}";

    /// <summary>
    /// Builds the URL route for a specific organization and user identifier.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="memberUserId">User identifier to remove.</param>
    /// <returns>URL route with the identifiers included.</returns>
    public static string BuildRoute(Guid organizationId, Guid memberUserId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{MemberUserId:guid}", memberUserId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// User identifier to remove.
    /// </summary>
    public Guid MemberUserId { get; set; }
}
