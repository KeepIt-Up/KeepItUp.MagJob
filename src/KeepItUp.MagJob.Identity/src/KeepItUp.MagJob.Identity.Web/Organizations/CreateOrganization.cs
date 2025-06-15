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
    : BaseEndpoint<CreateOrganizationRequest, Guid>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Post(CreateOrganizationRequest.Route);
        AllowAnonymous();
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
    protected override async Task<Guid> HandleEndpointAsync(CreateOrganizationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateOrganizationCommand
        {
            Name = req.Name,
            Description = req.Description,
            OwnerId = userId
        };

        return await mediator.Send(command, ct);
    }
}
