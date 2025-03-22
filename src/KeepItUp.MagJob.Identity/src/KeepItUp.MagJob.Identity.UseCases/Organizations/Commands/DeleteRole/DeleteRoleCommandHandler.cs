using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;

/// <summary>
/// Handler dla komendy DeleteRoleCommand.
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IRepository<Organization> _repository;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeleteRoleCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public DeleteRoleCommandHandler(
        IRepository<Organization> repository,
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
            var organization = await _repository.FirstOrDefaultAsync(
                new OrganizationWithRolesSpec(request.OrganizationId), cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do usuwania ról
            if (organization.OwnerId != request.UserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do usuwania ról w organizacji.");
                }
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
            var membersWithRole = organization.Members.Where(m => m.Roles.Any(r => r.Id == request.RoleId)).ToList();
            if (membersWithRole.Any())
            {
                return Result.Error("Nie można usunąć roli, która jest przypisana do członków organizacji.");
            }

            // Usuń rolę
            organization.RemoveRole(request.RoleId);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usunięto rolę o ID {RoleId} z organizacji o ID {OrganizationId}",
                request.RoleId, organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania roli");
            return Result.Error("Wystąpił błąd podczas usuwania roli: " + ex.Message);
        }
    }
} 
