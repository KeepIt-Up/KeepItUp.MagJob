using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do tworzenia organizacji.
/// </summary>
/// <remarks>
/// Tworzy nową organizację z podanymi danymi.
/// </remarks>
public class CreateOrganization(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<CreateOrganizationRequest, CreateOrganizationResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(CreateOrganizationRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("CreateOrganization")
            .Produces<CreateOrganizationResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Tworzy nową organizację";
            s.Description = "Tworzy nową organizację z podanymi danymi";
            s.ExampleRequest = new CreateOrganizationRequest { Name = "Nazwa organizacji", Description = "Opis organizacji" };
            s.ResponseExamples[201] = new CreateOrganizationResponse { Id = Guid.NewGuid(), Name = "Nazwa organizacji", Description = "Opis organizacji", OwnerId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie POST /api/organizations.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi utworzonej organizacji.</returns>
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

        // Zakładamy, że result.Value to Guid (identyfikator utworzonej organizacji)
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
