using FluentValidation;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.CreateInvitation;

/// <summary>
/// Validator for the CreateInvitationCommand.
/// </summary>
public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvitationCommandValidator"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    public CreateInvitationCommandValidator(
        IInvitationRepository invitationRepository,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
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

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Adres e-mail jest wymagany.")
            .EmailAddress().WithMessage("Podany adres e-mail jest nieprawidłowy.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .MustAsync(async (command, roleId, context, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, cancellationToken);
                return organization?.HasRole(roleId) ?? false;
            }).WithMessage("Podana rola nie istnieje w tej organizacji.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.")
            .MustAsync(async (command, userId, context, cancellationToken) =>
            {
                return !await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    userId,
                    cancellationToken);
            })
            .WithMessage("Nie można wysłać zaproszenia do użytkownika, który już jest członkiem organizacji.");

        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return !await _invitationRepository.HasPendingInvitationAsync(
                    command.OrganizationId,
                    command.Email,
                    cancellationToken);
            })
            .WithMessage("Istnieje już aktywne zaproszenie dla podanego adresu e-mail.");
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
