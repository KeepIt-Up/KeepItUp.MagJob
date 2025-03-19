using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;

/// <summary>
/// Walidator dla komendy RevokeRoleFromMemberCommand.
/// </summary>
public class RevokeRoleFromMemberCommandValidator : AbstractValidator<RevokeRoleFromMemberCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RevokeRoleFromMemberCommandValidator"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public RevokeRoleFromMemberCommandValidator(IOrganizationRepository organizationRepository, IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.MemberUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .MustAsync(async (command, roleId, context, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, cancellationToken);
                return organization?.Roles.Any(r => r.Id == roleId) == true;
            }).WithMessage("Rola o podanym identyfikatorze nie istnieje w tej organizacji.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika wykonującego operację jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik wykonujący operację nie istnieje.");

        // Sprawdzenie, czy użytkownik, któremu ma być odebrana rola, jest członkiem organizacji
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    command.MemberUserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik, któremu ma zostać odebrana rola, nie jest członkiem tej organizacji.");

        // Sprawdzenie, czy użytkownik wykonujący operację jest członkiem organizacji
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    command.RequestingUserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik wykonujący operację nie jest członkiem tej organizacji.");

        // Sprawdzenie, czy użytkownik ma przypisaną rolę, która ma zostać odebrana
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithMembersAndRolesAsync(command.OrganizationId, cancellationToken);
                var member = organization?.Members.FirstOrDefault(m => m.UserId == command.MemberUserId);
                return member?.Roles.Any(r => r.Id == command.RoleId) == true;
            })
            .WithMessage("Użytkownik nie ma przypisanej roli, która ma zostać odebrana.");
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
