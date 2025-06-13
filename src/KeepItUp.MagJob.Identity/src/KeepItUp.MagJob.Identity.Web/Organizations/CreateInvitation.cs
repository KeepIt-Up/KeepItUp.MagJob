using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to create an invitation to an organization.
/// </summary>
/// <remarks>
/// Creates a new invitation to an organization for the given email address.
/// </remarks>
public class CreateInvitation(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
  : Endpoint<CreateInvitationRequest, CreateInvitationResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(CreateInvitationRequest.Route);
        AllowAnonymous();
        Description(b => b
          .WithName("CreateInvitation")
          .Produces(201)
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(403)
          .ProducesProblem(404)
          .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Tworzy zaproszenie do organizacji";
            s.Description = "Tworzy nowe zaproszenie do organizacji dla podanego adresu email";
            s.ExampleRequest = new CreateInvitationRequest { OrganizationId = Guid.NewGuid(), Email = "example@example.com", RoleId = Guid.NewGuid() };
            s.ResponseExamples[201] = new CreateInvitationResponse { Id = Guid.NewGuid(), Email = "example@example.com" };
        });
    }

    /// <summary>
    /// Handles the POST /api/organizations/{organizationId}/invitations request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the identifier of the created invitation.</returns>
    public override async Task HandleAsync(CreateInvitationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateInvitationCommand()
        {
            OrganizationId = req.OrganizationId,
            Email = req.Email,
            RoleId = req.RoleId,
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

        Response = new CreateInvitationResponse()
        {
            Id = result.Value,
            Email = req.Email
        };
    }
}
