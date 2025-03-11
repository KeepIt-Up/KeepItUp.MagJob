using Ardalis.Result;
using FastEndpoints;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do przypisania roli członkowi organizacji.
/// </summary>
/// <remarks>
/// Przypisuje rolę członkowi organizacji o podanym identyfikatorze.
/// </remarks>
public class AssignRoleToMemberEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<AssignRoleToMemberRequest, EmptyResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("api/organizations/{organizationId}/members/{memberUserId}/roles");
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("AssignRoleToMember")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Przypisuje rolę członkowi organizacji";
            s.Description = "Przypisuje rolę członkowi organizacji o podanym identyfikatorze";
            s.ExampleRequest = new AssignRoleToMemberRequest { 
                OrganizationId = Guid.NewGuid(), 
                MemberUserId = Guid.NewGuid(),
                RoleId = Guid.NewGuid()
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie POST /api/organizations/{organizationId}/members/{memberUserId}/roles.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Pusta odpowiedź w przypadku powodzenia.</returns>
    public override async Task HandleAsync(AssignRoleToMemberRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new AssignRoleToMemberCommand
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
/// Żądanie przypisania roli członkowi organizacji.
/// </summary>
public class AssignRoleToMemberRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Identyfikator użytkownika, któremu ma zostać przypisana rola.
    /// </summary>
    public Guid MemberUserId { get; set; }

    /// <summary>
    /// Identyfikator roli do przypisania.
    /// </summary>
    public Guid RoleId { get; set; }
} 
