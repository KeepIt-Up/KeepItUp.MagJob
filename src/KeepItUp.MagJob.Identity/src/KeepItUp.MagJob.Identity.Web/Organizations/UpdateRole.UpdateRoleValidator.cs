using System.Text.RegularExpressions;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania UpdateRoleRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class UpdateRoleValidator : Validator<UpdateRoleRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRoleValidator"/>.
    /// </summary>
    public UpdateRoleValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        // Walidacja identyfikatora roli
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");

        // Walidacja nazwy roli
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa roli jest wymagana.")
            .MaximumLength(50).WithMessage("Nazwa roli nie może przekraczać 50 znaków.");

        // Walidacja opisu roli (opcjonalny)
        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Opis roli nie może przekraczać 200 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Walidacja koloru roli (opcjonalny)
        RuleFor(x => x.Color)
            .Must(BeValidHexColor).WithMessage("Kolor musi być w prawidłowym formacie HEX (np. #FF5733).")
            .When(x => !string.IsNullOrEmpty(x.Color));
    }

    /// <summary>
    /// Sprawdza, czy podana wartość jest prawidłowym kolorem w formacie HEX.
    /// </summary>
    /// <param name="color">Kolor do sprawdzenia.</param>
    /// <returns>True, jeśli kolor jest prawidłowym formatem HEX; w przeciwnym razie false.</returns>
    private bool BeValidHexColor(string? color)
    {
        if (string.IsNullOrEmpty(color))
            return true;

        // Sprawdzenie, czy kolor jest w formacie HEX (#RRGGBB lub #RGB)
        return Regex.IsMatch(color, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }
}
