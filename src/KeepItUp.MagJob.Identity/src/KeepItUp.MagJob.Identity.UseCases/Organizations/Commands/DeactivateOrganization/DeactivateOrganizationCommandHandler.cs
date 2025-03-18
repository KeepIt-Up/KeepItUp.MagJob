using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;

/// <summary>
/// Handler dla komendy DeactivateOrganizationCommand.
/// </summary>
public class DeactivateOrganizationCommandHandler : IRequestHandler<DeactivateOrganizationCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<DeactivateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeactivateOrganizationCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public DeactivateOrganizationCommandHandler(
        IOrganizationRepository repository,
        ILogger<DeactivateOrganizationCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę DeactivateOrganizationCommand.
    /// </summary>
    /// <param name="request">Komenda DeactivateOrganizationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(DeactivateOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.Id}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do dezaktywacji organizacji
            if (organization.OwnerId != request.UserId)
            {
                return Result.Forbidden("Tylko właściciel organizacji może ją dezaktywować.");
            }

            // Dezaktywuj organizację
            organization.Deactivate();

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Dezaktywowano organizację o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dezaktywacji organizacji");
            return Result.Error("Wystąpił błąd podczas dezaktywacji organizacji: " + ex.Message);
        }
    }
}
