
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie odebrania roli członkowi organizacji.
/// </summary>
public class RevokeRoleFromMemberRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Members/{MemberUserId:guid}/Roles/{RoleId:guid}";
    public static string BuildRoute(Guid organizationId, Guid memberUserId, Guid roleId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{MemberUserId:guid}", memberUserId.ToString()).Replace("{RoleId:guid}", roleId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator użytkownika, któremu ma zostać odebrana rola.
    /// </summary>
    public Guid MemberUserId { get; set; }

    /// <summary>
    /// Identyfikator roli do odebrania.
    /// </summary>
    public Guid RoleId { get; set; }
}
