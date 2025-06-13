using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update a role in an organization.
/// </summary>
/// <remarks>
/// Updates a role in an organization with the given identifier.
/// </remarks>
public class UpdateRole(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<UpdateRoleRequest>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Put(UpdateRoleRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("UpdateRole")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Aktualizuje rolę w organizacji";
            s.Description = "Aktualizuje rolę w organizacji o podanym identyfikatorze";
            s.ExampleRequest = new UpdateRoleRequest
            {
                OrganizationId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Name = "Administrator",
                Description = "Rola administratora organizacji",
                Color = "#FF0000"
            };
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/roles/{roleId} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    public override async Task HandleAsync(UpdateRoleRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new UpdateRoleCommand
        {
            OrganizationId = req.OrganizationId,
            RoleId = req.RoleId,
            Name = req.Name,
            Description = req.Description,
            Color = req.Color,
            UserId = userId
        };

        var result = await mediator.Send(command, ct);

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

        if (result.Status == ResultStatus.Invalid)
        {
            foreach (var error in result.ValidationErrors)
            {
                AddError(error.ErrorMessage);
            }
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
