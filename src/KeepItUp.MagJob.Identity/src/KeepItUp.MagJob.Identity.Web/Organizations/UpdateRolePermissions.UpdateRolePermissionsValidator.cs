namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania UpdateRolePermissionsRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class UpdateRolePermissionsValidator : Validator<UpdateRolePermissionsRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRolePermissionsValidator"/>.
    /// </summary>
    public UpdateRolePermissionsValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        // Walidacja identyfikatora roli
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");

        // Walidacja listy uprawnień
        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Lista uprawnień nie może być null.");

        // Walidacja każdego uprawnienia na liście
        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Nazwa uprawnienia nie może być pusta.")
            .MaximumLength(50).WithMessage("Nazwa uprawnienia nie może przekraczać 50 znaków.");
    }
}
