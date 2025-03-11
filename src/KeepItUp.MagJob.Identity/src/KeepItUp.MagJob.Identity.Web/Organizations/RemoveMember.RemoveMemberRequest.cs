
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie usunięcia członka z organizacji.
/// </summary>
public class RemoveMemberRequest
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Members/{MemberUserId:guid}";
    public static string BuildRoute(Guid organizationId, Guid memberUserId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString()).Replace("{MemberUserId:guid}", memberUserId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator użytkownika do usunięcia.
    /// </summary>
    public Guid MemberUserId { get; set; }
} 
