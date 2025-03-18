namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie usunięcia członka z organizacji.
/// </summary>
public class RemoveMemberRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu usuwania członka organizacji.
    /// </summary>
    public const string Route = "/Organizations/{OrganizationId:guid}/Members/{MemberUserId:guid}";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora organizacji i identyfikatora użytkownika.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="memberUserId">Identyfikator użytkownika do usunięcia.</param>
    /// <returns>Ścieżka URL z uwzględnionymi identyfikatorami.</returns>
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
