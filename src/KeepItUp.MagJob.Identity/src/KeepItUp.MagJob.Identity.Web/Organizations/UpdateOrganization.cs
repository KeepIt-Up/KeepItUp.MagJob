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
    : Endpoint<UpdateOrganizationRequest, UpdateOrganizationResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Put(UpdateOrganizationRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("UpdateOrganization")
            .Produces<UpdateOrganizationResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
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
    public override async Task HandleAsync(UpdateOrganizationRequest req, CancellationToken ct)
    {
        try
        {
            var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

            var command = new UpdateOrganizationCommand
            {
                Id = req.Id,
                Name = req.Name,
                Description = req.Description,
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

            if (result.Status == ResultStatus.Invalid)
            {
                foreach (var error in result.ValidationErrors)
                {
                    AddError(error.ErrorMessage);
                }
                await SendErrorsAsync(400, ct);
                return;
            }

            Response = new UpdateOrganizationResponse
            {
                Id = req.Id,
                Name = req.Name,
                Description = req.Description,
                OwnerId = userGuid
            };

            await SendOkAsync(Response, ct);
        }
        catch (UnauthorizedAccessException)
        {
            AddError("Nie można zidentyfikować użytkownika");
            await SendErrorsAsync(401, ct);
        }
    }
}
