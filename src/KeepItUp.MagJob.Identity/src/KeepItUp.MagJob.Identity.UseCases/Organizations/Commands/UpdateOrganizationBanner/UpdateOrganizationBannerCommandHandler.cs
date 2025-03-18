using Ardalis.Result;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;

/// <summary>
/// Handler dla komendy UpdateOrganizationBannerCommand.
/// </summary>
public class UpdateOrganizationBannerCommandHandler : IRequestHandler<UpdateOrganizationBannerCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateOrganizationBannerCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationBannerCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationBannerCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateOrganizationBannerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę UpdateOrganizationBannerCommand.
    /// </summary>
    /// <param name="request">Komenda UpdateOrganizationBannerCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(UpdateOrganizationBannerCommand request, CancellationToken cancellationToken)
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
                    return Result.Forbidden("Brak uprawnień do aktualizacji bannera organizacji.");
                }
            }

            // Aktualizuj banner organizacji
            organization.UpdateBanner(request.BannerUrl);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano banner organizacji o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji bannera organizacji");
            return Result.Error("Wystąpił błąd podczas aktualizacji bannera organizacji: " + ex.Message);
        }
    }
}
