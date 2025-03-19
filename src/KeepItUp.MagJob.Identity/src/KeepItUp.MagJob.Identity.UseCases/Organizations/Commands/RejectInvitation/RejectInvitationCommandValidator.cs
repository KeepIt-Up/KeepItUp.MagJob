using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Walidator dla komendy RejectInvitationCommand.
/// </summary>
public class RejectInvitationCommandValidator : AbstractValidator<RejectInvitationCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RejectInvitationCommandValidator"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public RejectInvitationCommandValidator(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.InvitationId)
            .NotEmpty().WithMessage("Identyfikator zaproszenia jest wymagany.")
            .MustAsync(async (command, invitationId, context, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithInvitationsAsync(command.OrganizationId, cancellationToken);
                return organization?.Invitations.Any(i => i.Id == invitationId) == true;
            }).WithMessage("Zaproszenie o podanym identyfikatorze nie istnieje w tej organizacji.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token zaproszenia jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        // Sprawdzenie, czy zaproszenie jest aktywne i czy token jest poprawny
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithInvitationsAsync(command.OrganizationId, cancellationToken);
                var invitation = organization?.Invitations.FirstOrDefault(i => i.Id == command.InvitationId);

                if (invitation == null)
                {
                    return false;
                }

                // Sprawdzenie statusu zaproszenia
                if (invitation.Status != InvitationStatus.Pending)
                {
                    return false;
                }

                // Sprawdzenie tokenu
                return invitation.Token == command.Token;
            })
            .WithMessage("Zaproszenie jest nieaktywne lub token jest nieprawidłowy.");

        // Sprawdzenie, czy adres email zaproszenia pasuje do emaila użytkownika
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithInvitationsAsync(command.OrganizationId, cancellationToken);
                var invitation = organization?.Invitations.FirstOrDefault(i => i.Id == command.InvitationId);

                if (invitation == null)
                {
                    return false;
                }

                var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

                if (user == null)
                {
                    return false;
                }

                return string.Equals(invitation.Email, user.Email, StringComparison.OrdinalIgnoreCase);
            })
            .WithMessage("Zaproszenie nie jest skierowane do tego użytkownika.");
    }

    /// <summary>
    /// Sprawdza, czy organizacja o podanym identyfikatorze istnieje.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli organizacja istnieje; w przeciwnym razie false.</returns>
    private async Task<bool> OrganizationExists(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _organizationRepository.ExistsAsync(organizationId, cancellationToken);
    }

    /// <summary>
    /// Sprawdza, czy użytkownik o podanym identyfikatorze istnieje.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>True, jeśli użytkownik istnieje; w przeciwnym razie false.</returns>
    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.ExistsAsync(userId, cancellationToken);
    }
}
