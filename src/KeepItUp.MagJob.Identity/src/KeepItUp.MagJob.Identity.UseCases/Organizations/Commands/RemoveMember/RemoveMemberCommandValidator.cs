using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;

/// <summary>
/// Validator for the RemoveMemberCommand.
/// </summary>
public class RemoveMemberCommandValidator : AbstractValidator<RemoveMemberCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveMemberCommandValidator"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    public RemoveMemberCommandValidator(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.MemberUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika do usunięcia jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika wykonującego operację jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik wykonujący operację nie istnieje.");

        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    command.MemberUserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik, który ma zostać usunięty, nie jest członkiem tej organizacji.");

        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    command.RequestingUserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik wykonujący operację nie jest członkiem tej organizacji.");

        RuleFor(x => x)
            .Must(command => command.MemberUserId != command.RequestingUserId)
            .WithMessage("Nie można usunąć samego siebie z organizacji.");
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
