using FastEndpoints;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;

/// <summary>
/// Handler for the AssignRoleToMemberCommand.
/// </summary>
public class AssignRoleToMemberCommandHandler : IRequestHandler<AssignRoleToMemberCommand, Result<EmptyResponse>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<AssignRoleToMemberCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignRoleToMemberCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public AssignRoleToMemberCommandHandler(
        IOrganizationRepository repository,
        ILogger<AssignRoleToMemberCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the AssignRoleToMemberCommand.
    /// </summary>
    /// <param name="request">AssignRoleToMemberCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(AssignRoleToMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            if (organization.OwnerId != request.RequestingUserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.RequestingUserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do przypisywania ról w organizacji.");
                }
            }

            var member = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);
            if (member == null)
            {
                return Result.NotFound($"Użytkownik o ID {request.MemberUserId} nie jest członkiem organizacji.");
            }

            // Check if the member already has this role
            if (member.HasRole(request.RoleId))
            {
                return Result.Error($"Użytkownik o ID {request.MemberUserId} już ma przypisaną rolę o ID {request.RoleId}.");
            }

            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result.NotFound("Nie znaleziono roli.");
            }

            // Use direct SQL insert as a workaround for EF Core many-to-many tracking issues
            await _repository.AddRoleToMemberAsync(member.Id, request.RoleId, cancellationToken);

            _logger.LogInformation("Przypisano rolę o ID {RoleId} użytkownikowi o ID {UserId} w organizacji o ID {OrganizationId}",
                request.RoleId, request.MemberUserId, organization.Id);

            return Result.Success(new EmptyResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas przypisywania roli");
            return Result.Error("Wystąpił błąd podczas przypisywania roli: " + ex.Message);
        }
    }
}
