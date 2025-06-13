using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to delete an organization.
/// </summary>
/// <remarks>
/// Deactivates an organization with the given identifier.
/// </remarks>
public class DeleteOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<DeleteOrganizationRequest>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete(DeleteOrganizationRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("DeleteOrganization")
            .Produces(204)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Deactivates an organization";
            s.Description = "Deactivates an organization with the given identifier";
        });
    }

    /// <summary>
    /// Handles the DELETE /api/organizations/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Empty response in case of success.</returns>
    public override async Task HandleAsync(DeleteOrganizationRequest req, CancellationToken ct)
    {
        try
        {
            var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

            var command = new DeactivateOrganizationCommand
            {
                Id = req.Id,
                UserId = userGuid
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

            await SendNoContentAsync(ct);
        }
        catch (UnauthorizedAccessException)
        {
            AddError("Nie można zidentyfikować użytkownika");
            await SendErrorsAsync(401, ct);
        }
    }
}
