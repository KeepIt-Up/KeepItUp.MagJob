using System.Text.RegularExpressions;
using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;

/// <summary>
/// Walidator dla komendy UpdateRoleCommand.
/// </summary>
public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRoleCommandValidator"/>.
    /// </summary>
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa roli jest wymagana.")
            .MaximumLength(50).WithMessage("Nazwa roli nie może być dłuższa niż 50 znaków.");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Opis roli nie może być dłuższy niż 200 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Color)
            .Must(BeValidHexColor).WithMessage("Kolor musi być w formacie HEX (np. #FF0000).")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");
    }

    private bool BeValidHexColor(string? color)
    {
        if (string.IsNullOrEmpty(color))
            return true;

        // Sprawdź, czy kolor jest w formacie HEX (#RRGGBB lub #RGB)
        return Regex.IsMatch(color, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }
}
