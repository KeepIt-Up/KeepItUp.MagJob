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
    : Endpoint<GetOrganizationMembersRequest, PaginationResult<MemberDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetOrganizationMembersRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("GetOrganizationMembers")
            .Produces<PaginationResult<MemberDto>>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
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
    public override async Task HandleAsync(GetOrganizationMembersRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationMembersQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = userId,
            PaginationParameters = req.PaginationParameters
        };

        var result = await mediator.Send(query, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Forbidden)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}
