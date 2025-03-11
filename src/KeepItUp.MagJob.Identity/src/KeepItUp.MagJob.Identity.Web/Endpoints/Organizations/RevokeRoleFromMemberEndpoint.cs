using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do odebrania roli członkowi organizacji.
/// </summary>
/// <remarks>
/// Odbiera rolę członkowi organizacji o podanym identyfikatorze.
/// </remarks>
public class RevokeRoleFromMemberEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<RevokeRoleFromMemberRequest, EmptyResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete("api/organizations/{organizationId}/members/{memberUserId}/roles/{roleId}");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("RevokeRoleFromMember")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Odbiera rolę członkowi organizacji";
            s.Description = "Odbiera rolę członkowi organizacji o podanym identyfikatorze";
            s.ExampleRequest = new RevokeRoleFromMemberRequest { 
                OrganizationId = Guid.NewGuid(), 
                MemberUserId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie DELETE /api/organizations/{organizationId}/members/{memberUserId}/roles/{roleId}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(RevokeRoleFromMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = req.OrganizationId,
            MemberUserId = req.MemberUserId,
            RoleId = req.RoleId,
            RequestingUserId = userId
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

        await SendNoContentAsync(ct);
    }
}

/// <summary>
/// Żądanie odebrania roli członkowi organizacji.
/// </summary>
public class RevokeRoleFromMemberRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator użytkownika, któremu ma zostać odebrana rola.
    /// </summary>
    public Guid MemberUserId { get; set; }

    /// <summary>
    /// Identyfikator roli do odebrania.
    /// </summary>
    public Guid RoleId { get; set; }
} 
