using System.Text.RegularExpressions;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the UpdateRoleRequest.
/// </summary>
/// <remarks>
/// Implements basic technical validation of input data in the Web layer.
/// </remarks>
public class UpdateRoleValidator : Validator<UpdateRoleRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateRoleValidator"/> class.
    /// </summary>
    public UpdateRoleValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa roli jest wymagana.")
            .MaximumLength(50).WithMessage("Nazwa roli nie może przekraczać 50 znaków.");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Opis roli nie może przekraczać 200 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Color)
            .Must(BeValidHexColor).WithMessage("Kolor musi być w prawidłowym formacie HEX (np. #FF5733).")
            .When(x => !string.IsNullOrEmpty(x.Color));
    }

    /// <summary>
    /// Checks if the given value is a valid color in HEX format.
    /// </summary>
    /// <param name="color">Color to check.</param>
    /// <returns>True, if the color is a valid HEX format; otherwise false.</returns>
    private bool BeValidHexColor(string? color)
    {
        if (string.IsNullOrEmpty(color))
            return true;

        return Regex.IsMatch(color, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }
}
