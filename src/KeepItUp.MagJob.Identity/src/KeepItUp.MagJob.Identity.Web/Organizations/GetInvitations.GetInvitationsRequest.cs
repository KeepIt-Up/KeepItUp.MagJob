using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Żądanie dla endpointu GetInvitationsEndpoint.
/// </summary>
public class GetInvitationsRequest : PaginationRequest<InvitationDto>
{
    public const string Route = "/Organizations/{OrganizationId:guid}/Invitations";
    public static string BuildRoute(Guid organizationId) => Route.Replace("{OrganizationId:guid}", organizationId.ToString());

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }
}
