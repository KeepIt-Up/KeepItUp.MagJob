using System.Text.RegularExpressions;
using FluentValidation;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;

/// <summary>
/// Walidator dla komendy UpdateRoleCommand.
/// </summary>
public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRoleCommandValidator"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    public UpdateRoleCommandValidator(
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

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa roli jest wymagana.")
            .MaximumLength(50).WithMessage("Nazwa roli nie może być dłuższa niż 50 znaków.")
            .MustAsync(async (command, name, context, cancellationToken) =>
            {
                var organization = await _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, cancellationToken);
                return organization?.Roles.All(r => r.Id == command.RoleId || !string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)) != false;
            }).WithMessage("Rola o podanej nazwie już istnieje w tej organizacji.");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Opis roli nie może być dłuższy niż 200 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Color)
            .Must(BeValidHexColor).WithMessage("Kolor musi być w formacie HEX (np. #FF0000).")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

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
    }

    private bool BeValidHexColor(string? color)
    {
        if (string.IsNullOrEmpty(color))
            return true;

        // Sprawdź, czy kolor jest w formacie HEX (#RRGGBB lub #RGB)
        return Regex.IsMatch(color, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
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
