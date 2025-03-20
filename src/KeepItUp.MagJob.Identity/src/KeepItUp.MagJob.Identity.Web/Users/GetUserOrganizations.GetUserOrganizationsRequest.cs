using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.SharedKernel.Pagination;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Żądanie dla endpointu GetUserOrganizationsEndpoint.
/// </summary>
public class GetUserOrganizationsRequest : PaginationRequest<OrganizationDto>
{
    public const string Route = "/Users/{Id:guid}/Organizations";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }
}
