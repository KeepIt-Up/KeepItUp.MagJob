using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Handler dla komendy RejectInvitationCommand.
/// </summary>
public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Result>
{
    private readonly IRepository<Organization> _organizationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly ILogger<RejectInvitationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RejectInvitationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public RejectInvitationCommandHandler(
        IRepository<Organization> organizationRepository,
        IReadRepository<User> userRepository,
        ILogger<RejectInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
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

            // Odrzuć zaproszenie
            organization.RejectInvitation(invitation.Id);

            // Zapisz zmiany w repozytorium
            await _organizationRepository.UpdateAsync(organization, cancellationToken);
            await _organizationRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaproszenie o ID {InvitationId} zostało odrzucone przez użytkownika o ID {UserId}",
                invitation.Id, request.UserId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odrzucania zaproszenia");
            return Result.Error("Wystąpił błąd podczas odrzucania zaproszenia: " + ex.Message);
        }
    }
} 
