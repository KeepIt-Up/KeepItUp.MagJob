using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;

/// <summary>
/// Handler dla komendy <see cref="UpdateRolePermissionsCommand"/>.
/// </summary>
public class UpdateRolePermissionsCommandHandler(
    IOrganizationRepository organizationRepository,
    ILogger<UpdateRolePermissionsCommandHandler> logger)
    : IRequestHandler<UpdateRolePermissionsCommand, Result>
{
    /// <summary>
    /// Obsługuje komendę <see cref="UpdateRolePermissionsCommand"/>.
    /// </summary>
    /// <param name="request">Komenda.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobieramy organizację wraz z rolami
            var organization = await organizationRepository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            // Walidator powinien zapewnić, że organizacja istnieje
            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o identyfikatorze {request.OrganizationId}.");
            }

            // Sprawdzamy, czy użytkownik ma dostęp do organizacji (uprawnienia)
            if (!await organizationRepository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken) &&
                organization.OwnerId != request.UserId)
            {
                return Result.Forbidden("Brak dostępu do organizacji.");
            }

            // Pobieramy rolę
            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);

            // Walidator powinien zapewnić, że rola istnieje
            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o identyfikatorze {request.RoleId} w organizacji.");
            }

            // Aktualizujemy uprawnienia roli
            role.ClearPermissions();
            foreach (var permissionName in request.Permissions)
            {
                role.AddPermission(new Permission(permissionName));
            }

            // Zapisujemy zmiany
            await organizationRepository.UpdateAsync(organization, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Błąd podczas aktualizacji uprawnień roli: {Message}", ex.Message);
            return Result.Error("Wystąpił błąd podczas aktualizacji uprawnień roli.");
        }
    }
}
