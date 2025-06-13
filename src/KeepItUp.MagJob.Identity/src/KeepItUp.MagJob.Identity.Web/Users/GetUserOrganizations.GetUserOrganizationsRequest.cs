using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Request for the GetUserOrganizationsEndpoint.
/// </summary>
public class GetUserOrganizationsRequest : PaginationRequest<OrganizationDto>
{
    public const string Route = "/Users/{Id:guid}/Organizations";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; set; }
}
