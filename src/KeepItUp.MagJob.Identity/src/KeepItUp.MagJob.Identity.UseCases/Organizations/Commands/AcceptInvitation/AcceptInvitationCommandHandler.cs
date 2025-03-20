using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Handler dla komendy AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AcceptInvitationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public AcceptInvitationCommandHandler(
        IOrganizationRepository organizationRepository,
        ILogger<AcceptInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę AcceptInvitationCommand.
    /// </summary>
    /// <param name="request">Komenda AcceptInvitationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator członka organizacji.</returns>
    public async Task<Result<Guid>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z zaproszeniami
            var organization = await _organizationRepository.GetByIdWithInvitationsAsync(request.OrganizationId, cancellationToken);

            // Walidator powinien zapewnić, że organizacja istnieje
            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Zaakceptuj zaproszenie i dodaj użytkownika jako członka organizacji
            var member = organization.AcceptInvitation(request.InvitationId, request.UserId);

            // Zapisz zmiany
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Użytkownik {UserId} zaakceptował zaproszenie {InvitationId} do organizacji {OrganizationId}",
                request.UserId, request.InvitationId, request.OrganizationId);

            return Result<Guid>.Success(member.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas akceptowania zaproszenia {InvitationId} przez użytkownika {UserId} do organizacji {OrganizationId}",
                request.InvitationId, request.UserId, request.OrganizationId);
            return Result<Guid>.Error("Wystąpił błąd podczas akceptowania zaproszenia: " + ex.Message);
        }
    }
}
