using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;

/// <summary>
/// Handler dla komendy DeactivateOrganizationCommand.
/// </summary>
public class DeactivateOrganizationCommandHandler : IRequestHandler<DeactivateOrganizationCommand, Result>
{
    private readonly IRepository<Organization> _repository;
    private readonly ILogger<DeactivateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeactivateOrganizationCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public DeactivateOrganizationCommandHandler(
        IRepository<Organization> repository,
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
            var organization = await _repository.FirstOrDefaultAsync(
                new OrganizationByIdSpec(request.Id), cancellationToken);

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
            await _repository.SaveChangesAsync(cancellationToken);

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
