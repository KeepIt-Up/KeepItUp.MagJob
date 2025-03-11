using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;
using UseCasesInvitationDto = KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.InvitationDto;
using UseCasesRoleDto = KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.RoleDto;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do pobierania zaproszeń organizacji.
/// </summary>
/// <remarks>
/// Pobiera wszystkie zaproszenia organizacji o podanym identyfikatorze.
/// </remarks>
public class GetOrganizationInvitationsEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationInvitationsRequest, GetOrganizationInvitationsResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("api/organizations/{organizationId}/invitations");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("GetOrganizationInvitations")
            .Produces<GetOrganizationInvitationsResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Pobiera zaproszenia organizacji";
            s.Description = "Pobiera wszystkie zaproszenia organizacji o podanym identyfikatorze";
            s.ExampleRequest = new GetOrganizationInvitationsRequest { OrganizationId = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Obsługuje żądanie GET /api/organizations/{organizationId}/invitations.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z listą zaproszeń organizacji.</returns>
    public override async Task HandleAsync(GetOrganizationInvitationsRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationInvitationsQuery
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

        Response = new GetOrganizationInvitationsResponse
        {
            OrganizationId = req.OrganizationId,
            Invitations = result.Value.Select(MapInvitationDto).ToList()
        };

        await SendOkAsync(Response, ct);
    }

    /// <summary>
    /// Mapuje InvitationDto z warstwy UseCases na InvitationDto z warstwy Web.
    /// </summary>
    /// <param name="useCasesDto">InvitationDto z warstwy UseCases.</param>
    /// <returns>InvitationDto z warstwy Web.</returns>
    private static InvitationDto MapInvitationDto(UseCasesInvitationDto useCasesDto)
    {
        return new InvitationDto
        {
            Id = useCasesDto.Id,
            Email = useCasesDto.Email,
            Status = useCasesDto.Status,
            ExpiresAt = useCasesDto.ExpiresAt,
            IsExpired = useCasesDto.IsExpired,
            CreatedAt = useCasesDto.CreatedAt,
            CreatedBy = useCasesDto.CreatedBy,
            Role = useCasesDto.Role != null ? new RoleDto
            {
                Id = useCasesDto.Role.Id,
                Name = useCasesDto.Role.Name,
                Description = useCasesDto.Role.Description,
                Color = useCasesDto.Role.Color
            } : null
        };
    }
}

/// <summary>
/// Żądanie pobrania zaproszeń organizacji.
/// </summary>
public class GetOrganizationInvitationsRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }
}

/// <summary>
/// Odpowiedź z listą zaproszeń organizacji.
/// </summary>
public class GetOrganizationInvitationsResponse
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Lista zaproszeń organizacji.
    /// </summary>
    public List<InvitationDto> Invitations { get; set; } = new();
} 
