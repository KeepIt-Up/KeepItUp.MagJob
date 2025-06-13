using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Validator for the AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptInvitationCommandValidator"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    public AcceptInvitationCommandValidator(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.")
            .MustAsync(async (id, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdAsync(id, cancellationToken);
                return organization != null && organization.IsActive;
            }).WithMessage("Organizacja jest nieaktywna.");

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

        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return !await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    command.UserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik jest już członkiem tej organizacji.");

        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithInvitationsAsync(command.OrganizationId, cancellationToken);
                var invitation = organization?.Invitations.FirstOrDefault(i => i.Id == command.InvitationId);

                if (invitation == null)
                {
                    return false;
                }

                if (invitation.Status != InvitationStatus.Pending)
                {
                    return false;
                }

                return invitation.Token == command.Token;
            })
            .WithMessage("Zaproszenie jest nieaktywne lub token jest nieprawidłowy.");
    }

    /// <summary>
    /// Checks if an organization with the given identifier exists.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the organization exists; otherwise false.</returns>
    private async Task<bool> OrganizationExists(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _organizationRepository.ExistsAsync(organizationId, cancellationToken);
    }

    /// <summary>
    /// Checks if a user with the given identifier exists.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user exists; otherwise false.</returns>
    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.ExistsAsync(userId, cancellationToken);
    }
}
