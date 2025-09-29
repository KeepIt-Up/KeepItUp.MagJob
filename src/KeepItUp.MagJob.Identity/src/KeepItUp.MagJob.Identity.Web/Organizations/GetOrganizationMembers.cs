using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to get the members of an organization.
/// </summary>
/// <remarks>
/// Gets all members of an organization with the given identifier.
/// </remarks>
public class GetOrganizationMembers(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<GetOrganizationMembersRequest, PaginationResult<MemberDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetOrganizationMembersRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets the members of an organization";
            s.Description = "Gets all members of an organization with the given identifier";
            s.ExampleRequest = new GetOrganizationMembersRequest
            {
                OrganizationId = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<MemberDto>.Create()
            };
        });
    }

    /// <summary>
    /// Handles the GET /api/organizations/{organizationId}/members request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the list of members of the organization with pagination.</returns>
    protected override async Task<PaginationResult<MemberDto>> HandleEndpointAsync(GetOrganizationMembersRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationMembersQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = userId,
            PaginationParameters = req.PaginationParameters
        };

        return await mediator.Send(query, ct);
    }
}
