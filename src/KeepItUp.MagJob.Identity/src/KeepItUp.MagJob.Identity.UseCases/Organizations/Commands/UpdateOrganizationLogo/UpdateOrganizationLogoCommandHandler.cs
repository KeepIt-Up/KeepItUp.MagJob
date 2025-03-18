using Ardalis.Result;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;

/// <summary>
/// Handler dla komendy UpdateOrganizationLogoCommand.
/// </summary>
public class UpdateOrganizationLogoCommandHandler : IRequestHandler<UpdateOrganizationLogoCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateOrganizationLogoCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationLogoCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationLogoCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateOrganizationLogoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę UpdateOrganizationLogoCommand.
    /// </summary>
    /// <param name="request">Komenda UpdateOrganizationLogoCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(UpdateOrganizationLogoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do aktualizacji organizacji
            if (organization.OwnerId != request.UserId)
            {
                var isMember = organization.Members.Any(m => m.UserId == request.UserId &&
                    m.Roles.Any(r => r.Name == "Admin"));

                if (!isMember)
                {
                    return Result.Forbidden("Brak uprawnień do aktualizacji logo organizacji.");
                }
            }

            // Aktualizuj logo organizacji
            organization.UpdateLogo(request.LogoUrl);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano logo organizacji o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji logo organizacji");
            return Result.Error("Wystąpił błąd podczas aktualizacji logo organizacji: " + ex.Message);
        }
    }
}
