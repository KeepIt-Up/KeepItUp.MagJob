using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;

/// <summary>
/// Handler dla komendy DeleteRoleCommand.
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeleteRoleCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public DeleteRoleCommandHandler(
        IOrganizationRepository repository,
        ILogger<DeleteRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę DeleteRoleCommand.
    /// </summary>
    /// <param name="request">Komenda DeleteRoleCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }


            // Znajdź rolę do usunięcia
            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            // Sprawdź, czy rola nie jest jedną z domyślnych ról systemowych
            if (role.Name is "Admin" or "Member" or "Guest")
            {
                return Result.Error("Nie można usunąć domyślnej roli systemowej.");
            }

            // Sprawdź, czy rola nie jest przypisana do żadnego członka
            var membersWithRole = organization.Members.Where(m => m.HasRole(request.RoleId)).ToList();
            if (membersWithRole.Any())
            {
                return Result.Error("Nie można usunąć roli, która jest przypisana do członków organizacji.");
            }

            // Usuń rolę używając nowej metody repozytorium
            await _repository.DeleteRoleAsync(request.OrganizationId, request.RoleId, cancellationToken);

            _logger.LogInformation("Usunięto rolę o ID {RoleId} z organizacji o ID {OrganizationId}",
                request.RoleId, request.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania roli");
            return Result.Error("Wystąpił błąd podczas usuwania roli: " + ex.Message);
        }
    }
}
