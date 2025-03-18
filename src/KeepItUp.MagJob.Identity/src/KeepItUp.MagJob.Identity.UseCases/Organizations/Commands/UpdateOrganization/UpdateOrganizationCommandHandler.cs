using Ardalis.Result;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Handler dla komendy UpdateOrganizationCommand.
/// </summary>
public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateOrganizationCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę UpdateOrganizationCommand.
    /// </summary>
    /// <param name="request">Komenda UpdateOrganizationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.Id, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.Id}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do aktualizacji organizacji
            if (organization.OwnerId != request.UserId)
            {
                var isMember = organization.Members.Any(m => m.UserId == request.UserId &&
                    m.Roles.Any(r => r.Name == "Admin"));

                if (!isMember)
                {
                    return Result.Forbidden("Brak uprawnień do aktualizacji organizacji.");
                }
            }

            // Sprawdź, czy nazwa organizacji nie jest już używana przez inną organizację
            if (organization.Name != request.Name)
            {
                var existingOrganization = await _repository.GetByNameAsync(request.Name, cancellationToken);

                if (existingOrganization != null && existingOrganization.Id != request.Id)
                {
                    return Result.Error("Organizacja o podanej nazwie już istnieje.");
                }
            }

            // Aktualizuj organizację
            organization.Update(request.Name, request.Description);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano organizację o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji organizacji");
            return Result.Error("Wystąpił błąd podczas aktualizacji organizacji: " + ex.Message);
        }
    }
}
