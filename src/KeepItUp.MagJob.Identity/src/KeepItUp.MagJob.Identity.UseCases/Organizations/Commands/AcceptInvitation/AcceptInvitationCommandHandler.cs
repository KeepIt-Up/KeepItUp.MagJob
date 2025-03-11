using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Handler dla komendy AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AcceptInvitationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public AcceptInvitationCommandHandler(
        IRepository<Organization> organizationRepository,
        IReadRepository<User> userRepository,
        ILogger<AcceptInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę AcceptInvitationCommand.
    /// </summary>
    /// <param name="request">Komenda AcceptInvitationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz użytkownika
            var user = await _userRepository.FirstOrDefaultAsync(
                new UserByIdSpec(request.UserId), cancellationToken);

            if (user == null)
            {
                return Result.NotFound($"Nie znaleziono użytkownika o ID {request.UserId}.");
            }

            // Pobierz organizację z zaproszeniem
            var organization = await _organizationRepository.FirstOrDefaultAsync(
                new OrganizationWithInvitationSpec(request.InvitationId), cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono zaproszenia o ID {request.InvitationId}.");
            }

            // Znajdź zaproszenie
            var invitation = organization.Invitations.FirstOrDefault(i => i.Id == request.InvitationId);
            if (invitation == null)
            {
                return Result.NotFound($"Nie znaleziono zaproszenia o ID {request.InvitationId}.");
            }

            // Sprawdź, czy token jest poprawny
            if (invitation.Token != request.Token)
            {
                return Result.Unauthorized("Nieprawidłowy token zaproszenia.");
            }

            // Sprawdź, czy zaproszenie jest aktywne
            if (invitation.Status != InvitationStatus.Pending || invitation.IsExpired)
            {
                return Result.Error("Zaproszenie wygasło lub zostało już zaakceptowane/odrzucone.");
            }

            // Sprawdź, czy adres e-mail użytkownika zgadza się z adresem e-mail zaproszenia
            if (user.Email != invitation.Email)
            {
                return Result.Unauthorized("Adres e-mail użytkownika nie zgadza się z adresem e-mail zaproszenia.");
            }

            // Sprawdź, czy użytkownik jest już członkiem organizacji
            var isMember = organization.Members.Any(m => m.UserId == request.UserId);
            if (isMember)
            {
                return Result.Error("Użytkownik jest już członkiem organizacji.");
            }

            // Akceptuj zaproszenie
            organization.AcceptInvitation(invitation.Id, request.UserId);

            // Zapisz zmiany w repozytorium
            await _organizationRepository.UpdateAsync(organization, cancellationToken);
            await _organizationRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaproszenie o ID {InvitationId} zostało zaakceptowane przez użytkownika o ID {UserId}",
                invitation.Id, request.UserId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas akceptacji zaproszenia");
            return Result.Error("Wystąpił błąd podczas akceptacji zaproszenia: " + ex.Message);
        }
    }
} 
