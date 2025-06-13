using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;
using KeepItUp.MagJob.Identity.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint to get the organizations of a user.
/// </summary>
/// <remarks>
/// Gets all organizations the user belongs to.
/// </remarks>
[Authorize]
public class GetUserOrganizations(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetUserOrganizationsRequest, PaginationResult<OrganizationDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetUserOrganizationsRequest.Route);
        Description(b => b
            .WithName("GetUserOrganizations")
            .Produces<PaginationResult<OrganizationDto>>(200));
        Summary(s =>
        {
            s.Summary = "Gets the organizations of a user";
            s.Description = "Gets all organizations the user belongs to";
            s.ExampleRequest = new GetUserOrganizationsRequest
            {
                Id = Guid.NewGuid(),
                PaginationParameters = PaginationParameters<OrganizationDto>.Create()
            };
        });
    }

    /// <summary>
    /// Handles the GET /api/users/{id}/organizations request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the list of organizations the user belongs to.</returns>
    public override async Task HandleAsync(GetUserOrganizationsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetCurrentUserId();

        if (userId == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserOrganizationsQuery
        {
            UserId = req.Id,
            PaginationParameters = req.PaginationParameters
        };

        var result = await mediator.Send(query, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}
