using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do pobierania członków organizacji.
/// </summary>
/// <remarks>
/// Pobiera wszystkich członków organizacji o podanym identyfikatorze.
/// </remarks>
public class GetOrganizationMembersEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationMembersRequest, GetOrganizationMembersResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("api/organizations/{organizationId}/members");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetOrganizationMembers")
            .Produces<GetOrganizationMembersResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Pobiera członków organizacji";
            s.Description = "Pobiera wszystkich członków organizacji o podanym identyfikatorze";
            s.ExampleRequest = new GetOrganizationMembersRequest { OrganizationId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/organizations/{organizationId}/members.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z listą członków organizacji.</returns>
    public override async Task HandleAsync(GetOrganizationMembersRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationMembersQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = userId
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

        Response = new GetOrganizationMembersResponse
        {
            OrganizationId = req.OrganizationId,
            MembersList = result.Value.ToList()
        };

        await SendOkAsync(Response, ct);
    }
}

/// <summary>
/// Żądanie pobrania członków organizacji.
/// </summary>
public class GetOrganizationMembersRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }
}

/// <summary>
/// Odpowiedź z listą członków organizacji.
/// </summary>
public class GetOrganizationMembersResponse
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Lista członków organizacji.
    /// </summary>
    public List<KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.MemberDto> MembersList { get; set; } = new();
} 
