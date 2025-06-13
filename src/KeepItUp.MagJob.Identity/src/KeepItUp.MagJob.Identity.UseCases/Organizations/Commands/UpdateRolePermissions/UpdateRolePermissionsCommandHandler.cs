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
    : IRequestHandler<UpdateRolePermissionsCommand, Result>
{
    /// <summary>
    /// Handles the UpdateRolePermissionsCommand.
    /// </summary>
    /// <param name="request">UpdateRolePermissionsCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
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
