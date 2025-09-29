using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitations;

namespace KeepItUp.MagJob.Identity.Web.Invitations;

/// <summary>
/// Endpoint to get invitations to an organization.
/// </summary>
/// <remarks>
/// Gets all invitations to an organization with the given identifier.
/// </remarks>
public class GetInvitationsEndpoint(IMediator mediator)
    : BaseEndpoint<GetInvitationsRequest, PaginationResult<InvitationDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetInvitationsRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get invitations to an organization";
            s.Description = "Get all invitations to an organization with the given identifier";
            s.ExampleRequest = new GetInvitationsRequest
            {
                OrganizationId = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<InvitationDto>.Create()
            };
        });
    }

    /// <summary>
    /// Handles the GET /api/organizations/{id}/invitations request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the list of invitations to the organization with pagination.</returns>
    protected override async Task<PaginationResult<InvitationDto>> HandleEndpointAsync(GetInvitationsRequest req, CancellationToken ct)
    {

        var query = new GetInvitationsQuery
        {
            OrganizationId = req.OrganizationId,
            Email = req.Email,
            PaginationParameters = req.PaginationParameters
        };

        return await mediator.Send(query, ct);
    }
}
