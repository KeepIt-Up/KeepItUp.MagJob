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
public class GetUserOrganizations(IMediator mediator)
    : BaseEndpoint<GetUserOrganizationsRequest, PaginationResult<OrganizationDto>>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetUserOrganizationsRequest.Route);
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
    protected override async Task<PaginationResult<OrganizationDto>> HandleEndpointAsync(GetUserOrganizationsRequest req, CancellationToken ct)
    {
        var query = new GetUserOrganizationsQuery
        {
            UserId = req.Id,
            PaginationParameters = req.PaginationParameters
        };

        return await mediator.Send(query, ct);
    }
}
