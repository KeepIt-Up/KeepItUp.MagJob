using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;

/// <summary>
/// Walidator dla komendy <see cref="UpdateRolePermissionsCommand"/>.
/// </summary>
public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRolePermissionsCommandValidator"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public UpdateRolePermissionsCommandValidator(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .MustAsync(async (command, roleId, context, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, cancellationToken);
                return organization?.Roles.Any(r => r.Id == roleId) == true;
            }).WithMessage("Rola o podanym identyfikatorze nie istnieje w tej organizacji.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Lista uprawnień nie może być null.");

        // Sprawdzenie, czy użytkownik wykonujący operację jest członkiem organizacji
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.OrganizationId,
                    command.UserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik wykonujący operację nie jest członkiem tej organizacji.");

        // Walidacja każdego uprawnienia z listy
        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Nazwa uprawnienia nie może być pusta.")
            .MaximumLength(100).WithMessage("Nazwa uprawnienia nie może być dłuższa niż 100 znaków.");
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
