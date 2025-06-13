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
    : Endpoint<CreateRoleRequest, CreateRoleResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(CreateRoleRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("CreateRole")
            .Produces(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
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
    public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
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

        Response = new CreateRoleResponse
        {
            Id = result.Value,
            Name = req.Name
        };
    }
}
