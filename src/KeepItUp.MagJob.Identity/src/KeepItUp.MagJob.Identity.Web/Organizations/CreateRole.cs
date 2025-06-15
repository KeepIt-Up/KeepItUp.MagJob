using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateRole;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to create a new role in an organization.
/// </summary>
/// <remarks>
/// Creates a new role in an organization with the given identifier.
/// </remarks>
public class CreateRole(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : BaseEndpoint<CreateRoleRequest, Guid>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Post(CreateRoleRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Creates a new role in an organization";
            s.Description = "Creates a new role in an organization with the given identifier";
            s.ExampleRequest = new CreateRoleRequest
            {
                OrganizationId = Guid.NewGuid(),
                Name = "Administrator",
                Description = "Rola administratora organizacji",
                Color = "#FF0000"
            };
            s.ResponseExamples[201] = new CreateRoleResponse
            {
                Id = Guid.NewGuid(),
                Name = "Administrator"
            };
        });
    }

    /// <summary>
    /// Handles the POST /api/organizations/{organizationId}/roles request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the identifier of the created role.</returns>
    protected override async Task<Guid> HandleEndpointAsync(CreateRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateRoleCommand
        {
            OrganizationId = req.OrganizationId,
            Name = req.Name,
            Description = req.Description,
            Color = req.Color,
            UserId = userId
        };

        return await mediator.Send(command, ct);
    }
}
