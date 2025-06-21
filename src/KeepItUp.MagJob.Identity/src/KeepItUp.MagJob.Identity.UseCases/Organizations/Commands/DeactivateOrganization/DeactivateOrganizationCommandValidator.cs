using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;

/// <summary>
/// Walidator dla komendy DeactivateOrganizationCommand.
/// </summary>
public class DeactivateOrganizationCommandValidator : AbstractValidator<DeactivateOrganizationCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeactivateOrganizationCommandValidator"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public DeactivateOrganizationCommandValidator(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        // Sprawdzenie, czy użytkownik jest członkiem organizacji
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.Id,
                    command.UserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik wykonujący operację nie jest członkiem tej organizacji.");

        // Sprawdzenie, czy organizacja jest aktywna
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdAsync(command.Id, cancellationToken);
                return organization != null && organization.IsActive;
            })
            .WithMessage("Organizacja jest już nieaktywna.");
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
