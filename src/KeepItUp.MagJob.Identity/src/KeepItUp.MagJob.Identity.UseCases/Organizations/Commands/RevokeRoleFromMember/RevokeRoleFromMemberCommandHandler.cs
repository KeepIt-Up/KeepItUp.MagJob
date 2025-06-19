using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;

/// <summary>
/// Handler for the RevokeRoleFromMemberCommand.
/// </summary>
public class RevokeRoleFromMemberCommandHandler : IRequestHandler<RevokeRoleFromMemberCommand, Result<EmptyResponse>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<RevokeRoleFromMemberCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeRoleFromMemberCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public RevokeRoleFromMemberCommandHandler(
        IOrganizationRepository repository,
        ILogger<RevokeRoleFromMemberCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the RevokeRoleFromMemberCommand.
    /// </summary>
    /// <param name="request">RevokeRoleFromMemberCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(RevokeRoleFromMemberCommand request, CancellationToken cancellationToken)
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
                    return Result.Forbidden("Brak uprawnień do odbierania ról w organizacji.");
                }
            }

            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);

            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            var member = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);

            if (member == null)
            {
                return Result.NotFound($"Użytkownik o ID {request.MemberUserId} nie jest członkiem organizacji.");
            }

            if (!member.HasRole(request.RoleId))
            {
                return Result.Error($"Użytkownik o ID {request.MemberUserId} nie ma przypisanej roli o ID {request.RoleId}.");
            }

            if (request.MemberUserId == organization.OwnerId && role.Name == "Admin")
            {
                return Result.Error("Nie można odebrać roli właściciela organizacji.");
            }

            if (member.Roles.Count == 1)
            {
                return Result.Error("Nie można odebrać ostatniej roli użytkownikowi. Użytkownik musi mieć przypisaną co najmniej jedną rolę.");
            }

            member.RemoveRole(request.RoleId);

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Odebrano rolę o ID {RoleId} użytkownikowi o ID {UserId} w organizacji o ID {OrganizationId}",
                request.RoleId, request.MemberUserId, organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odbierania roli");
            return Result.Error("Wystąpił błąd podczas odbierania roli: " + ex.Message);
        }
    }
}
