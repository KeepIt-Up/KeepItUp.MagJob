using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Walidator dla komendy UpdateOrganizationCommand.
/// </summary>
public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationCommandValidator"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public UpdateOrganizationCommandValidator(IOrganizationRepository organizationRepository, IUserRepository userRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .MustAsync(OrganizationExists).WithMessage("Organizacja o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może być dłuższa niż 100 znaków.")
            .MustAsync(async (command, name, context, cancellationToken) =>
            {
                var org = await _organizationRepository.GetByNameAsync(name, cancellationToken);
                return org == null || org.Id == command.Id; // Nazwa może być taka sama jak obecna
            }).WithMessage("Organizacja o tej nazwie już istnieje.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może być dłuższy niż 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        // Dodaj regułę walidacji dla członkostwa użytkownika w organizacji
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _organizationRepository.HasMemberAsync(
                    command.Id,
                    command.UserId,
                    cancellationToken);
            })
            .WithMessage("Użytkownik nie jest członkiem tej organizacji.");
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
