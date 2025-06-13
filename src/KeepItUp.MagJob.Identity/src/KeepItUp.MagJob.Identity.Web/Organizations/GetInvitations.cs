using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to get invitations to an organization.
/// </summary>
/// <remarks>
/// Gets all invitations to an organization with the given identifier.
/// </remarks>
public class GetInvitationsEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetInvitationsRequest, PaginationResult<InvitationDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetInvitationsRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("GetInvitations")
            .Produces<PaginationResult<InvitationDto>>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera zaproszenia do organizacji";
            s.Description = "Pobiera wszystkie zaproszenia do organizacji o podanym identyfikatorze";
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
    public override async Task HandleAsync(GetInvitationsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationInvitationsQuery
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
