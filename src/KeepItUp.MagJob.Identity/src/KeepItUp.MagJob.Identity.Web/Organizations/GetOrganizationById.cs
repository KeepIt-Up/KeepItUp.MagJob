using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to get an organization by its identifier.
/// </summary>
/// <remarks>
/// Gets an organization with the given identifier.
/// </remarks>
public class GetOrganizationById(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationByIdRequest, GetOrganizationByIdResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(GetOrganizationByIdRequest.Route);
        Description(b => b
            .WithName("GetOrganization")
            .Produces<GetOrganizationByIdResponse>(200));
        Summary(s =>
        {
            s.Summary = "Gets an organization";
            s.Description = "Gets an organization with the given identifier";
        });
    }

    /// <summary>
    /// Handles the GET /api/organizations/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the organization data.</returns>
    public override async Task HandleAsync(GetOrganizationByIdRequest req, CancellationToken ct)
    {
        try
        {
            var userGuid = currentUserAccessor.GetRequiredCurrentUserId();

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = req.Id,
                UserId = userGuid
            };

            var result = await mediator.Send(query, ct);

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

            Response = new GetOrganizationByIdResponse
            {
                Id = result.Value.Id,
                Name = result.Value.Name,
                Description = result.Value.Description,
                OwnerId = result.Value.OwnerId,
                IsOwner = result.Value.OwnerId == userGuid,
                MemberCount = 0, // Tymczasowo ustawiamy na 0
                LogoUrl = result.Value.LogoUrl,
                BannerUrl = result.Value.BannerUrl
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
