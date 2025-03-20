using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;

/// <summary>
/// Handler dla komendy CreateInvitationCommand.
/// </summary>
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<CreateInvitationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateInvitationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public CreateInvitationCommandHandler(
        IOrganizationRepository organizationRepository,
        ILogger<CreateInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę CreateInvitationCommand.
    /// </summary>
    /// <param name="request">Komenda CreateInvitationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator utworzonego zaproszenia.</returns>
    public async Task<Result<Guid>> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _organizationRepository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            // Walidator powinien zapewnić, że organizacja istnieje
            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Utwórz nowe zaproszenie
            var invitation = organization.CreateInvitation(request.Email, request.RoleId);

            // Zapisz zmiany w repozytorium
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Utworzono zaproszenie o ID {InvitationId} dla adresu e-mail {Email} do organizacji {OrganizationId}",
                invitation.Id, request.Email, organization.Id);

            return Result<Guid>.Success(invitation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia zaproszenia");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia zaproszenia: " + ex.Message);
        }
    }
}
