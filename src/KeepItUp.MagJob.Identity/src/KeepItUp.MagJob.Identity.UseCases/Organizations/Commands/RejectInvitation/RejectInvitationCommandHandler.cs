using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Handler dla komendy RejectInvitationCommand.
/// </summary>
public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Result>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<RejectInvitationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RejectInvitationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public RejectInvitationCommandHandler(
        IOrganizationRepository organizationRepository,
        ILogger<RejectInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę RejectInvitationCommand.
    /// </summary>
    /// <param name="request">Komenda RejectInvitationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z zaproszeniami
            var organization = await _organizationRepository.GetByIdWithInvitationsAsync(request.OrganizationId, cancellationToken);

            // Walidator powinien zapewnić, że organizacja istnieje
            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Odrzuć zaproszenie
            organization.RejectInvitation(request.InvitationId);

            // Zapisz zmiany
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Odrzucono zaproszenie {InvitationId} do organizacji {OrganizationId}",
                request.InvitationId, request.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odrzucania zaproszenia {InvitationId} do organizacji {OrganizationId}",
                request.InvitationId, request.OrganizationId);
            return Result.Error("Wystąpił błąd podczas odrzucania zaproszenia: " + ex.Message);
        }
    }
}
