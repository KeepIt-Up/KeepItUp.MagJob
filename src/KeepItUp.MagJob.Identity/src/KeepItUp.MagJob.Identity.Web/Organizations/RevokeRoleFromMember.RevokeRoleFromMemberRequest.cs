
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Request to revoke a role from a member of an organization.
/// </summary>
public class RevokeRoleFromMemberRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Members/{MemberUserId:guid}/Roles/{RoleId:guid}";
    public static string BuildRoute(Guid organizationId, Guid memberUserId, Guid roleId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{MemberUserId:guid}", memberUserId.ToString()).Replace("{RoleId:guid}", roleId.ToString());

    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// User identifier to revoke the role from.
    /// </summary>
    public Guid MemberUserId { get; set; }

    /// <summary>
    /// Role identifier to revoke.
    /// </summary>
    public Guid RoleId { get; set; }
}
