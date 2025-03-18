using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;

/// <summary>
/// Handler dla komendy UpdateRoleCommand.
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRoleCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public UpdateRoleCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę UpdateRoleCommand.
    /// </summary>
    /// <param name="request">Komenda UpdateRoleCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do aktualizacji ról
            if (organization.OwnerId != request.UserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do aktualizacji ról w organizacji.");
                }
            }

            // Znajdź rolę do aktualizacji
            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            // Sprawdź, czy nazwa roli nie jest już używana przez inną rolę
            if (organization.Roles.Any(r => r.Name == request.Name && r.Id != request.RoleId))
            {
                return Result.Error($"Rola o nazwie '{request.Name}' już istnieje w organizacji.");
            }

            // Sprawdź, czy rola nie jest jedną z domyślnych ról systemowych
            if (role.Name is "Admin" or "Member" or "Guest" && role.Name != request.Name)
            {
                return Result.Error("Nie można zmienić nazwy domyślnej roli systemowej.");
            }

            // Aktualizuj rolę
            role.Update(
                request.Name,
                request.Description,
                request.Color);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano rolę o ID {RoleId} w organizacji o ID {OrganizationId}",
                request.RoleId, organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji roli");
            return Result.Error("Wystąpił błąd podczas aktualizacji roli: " + ex.Message);
        }
    }
}
