using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to get an organization by its identifier.
/// </summary>
/// <remarks>
/// Gets an organization with the given identifier.
/// </remarks>
public class GetOrganizationById(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<GetOrganizationByIdRequest, OrganizationDto>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetOrganizationByIdRequest.Route);
        Summary(s =>
        {
            s.Summary = "Gets an organization";
            s.Description = "Gets an organization with the given identifier";
        });
    }

    /// <summary>
    /// Handles the GET /api/organizations/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the organization data.</returns>
    protected override async Task<OrganizationDto> HandleEndpointAsync(GetOrganizationByIdRequest req, CancellationToken ct)
    {
        var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = req.Id,
            UserId = userGuid
        };

        return await mediator.Send(query, ct);
    }
}
