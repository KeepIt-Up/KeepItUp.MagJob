using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to create an organization.
/// </summary>
/// <remarks>
/// Creates a new organization with the given data.
/// </remarks>
public class CreateOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<CreateOrganizationRequest, CreateOrganizationResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(CreateOrganizationRequest.Route);
        Description(b => b
            .WithName("CreateOrganization")
            .Produces<CreateOrganizationResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Creates a new organization";
            s.Description = "Creates a new organization with the given data";
            s.ExampleRequest = new CreateOrganizationRequest { Name = "Nazwa organizacji", Description = "Opis organizacji" };
            s.ResponseExamples[201] = new CreateOrganizationResponse { Id = Guid.NewGuid(), Name = "Nazwa organizacji", Description = "Opis organizacji", OwnerId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the POST /api/organizations request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the data of the created organization.</returns>
    public override async Task HandleAsync(CreateOrganizationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateOrganizationCommand
        {
            Name = req.Name,
            Description = req.Description,
            OwnerId = userId
        };

        var result = await mediator.Send(command, ct);

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

        var organizationId = result.Value;

        Response = new CreateOrganizationResponse
        {
            Id = organizationId,
            Name = req.Name,
            Description = req.Description,
            OwnerId = userId
        };
    }
}
