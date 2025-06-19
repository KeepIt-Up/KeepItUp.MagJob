using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;

/// <summary>
/// Handler for the UpdateRolePermissionsCommand.
/// </summary>
public class UpdateRolePermissionsCommandHandler(
    IOrganizationRepository organizationRepository,
    ILogger<UpdateRolePermissionsCommandHandler> logger)
    : IRequestHandler<UpdateRolePermissionsCommand, Result<EmptyResponse>>
{
    /// <summary>
    /// Handles the UpdateRolePermissionsCommand.
    /// </summary>
    /// <param name="request">UpdateRolePermissionsCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await organizationRepository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o identyfikatorze {request.OrganizationId}.");
            }

            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);

            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o identyfikatorze {request.RoleId} w organizacji.");
            }

            bool isOwner = organization.OwnerId == request.UserId;
            bool isAdmin = false;

            var organizationWithMembers = await organizationRepository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);
            if (organizationWithMembers != null)
            {
                var member = organizationWithMembers.Members.FirstOrDefault(m => m.UserId == request.UserId);
                if (member != null)
                {
                    isAdmin = member.Roles.Any(r => r.Name == "Admin");
                }
            }

            if (!isOwner && !isAdmin)
            {
                return Result.Forbidden("Brak uprawnień do aktualizacji uprawnień roli.");
            }

            await organizationRepository.UpdateRolePermissionsAsync(request.RoleId, request.Permissions, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Błąd podczas aktualizacji uprawnień roli: {Message}", ex.Message);
            return Result.Error("Wystąpił błąd podczas aktualizacji uprawnień roli.");
        }
    }
}
