using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint to update an organization.
/// </summary>
/// <remarks>
/// Updates an existing organization with the given identifier.
/// </remarks>
public class UpdateOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<UpdateOrganizationRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateOrganizationRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Updates an existing organization";
            s.Description = "Updates an existing organization with the given identifier";
            s.ExampleRequest = new UpdateOrganizationRequest { Id = Guid.NewGuid(), Name = "Nowa nazwa organizacji", Description = "Nowy opis organizacji" };
            s.ResponseExamples[200] = new UpdateOrganizationResponse { Id = Guid.NewGuid(), Name = "Nowa nazwa organizacji", Description = "Nowy opis organizacji", OwnerId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response with the updated organization data.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(UpdateOrganizationRequest req, CancellationToken ct)
    {
        var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new UpdateOrganizationCommand
        {
            Id = req.Id,
            Name = req.Name,
            Description = req.Description,
            UserId = userGuid
        };

        return await mediator.Send(command, ct);
    }
}
