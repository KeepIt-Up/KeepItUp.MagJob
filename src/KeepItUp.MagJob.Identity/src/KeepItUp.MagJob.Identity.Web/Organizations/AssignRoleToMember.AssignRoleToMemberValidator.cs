namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania AssignRoleToMemberRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class AssignRoleToMemberValidator : Validator<AssignRoleToMemberRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AssignRoleToMemberValidator"/>.
    /// </summary>
    public AssignRoleToMemberValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        // Walidacja identyfikatora użytkownika
        RuleFor(x => x.MemberUserId)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator użytkownika nie może być pusty (Guid.Empty).");

        // Walidacja identyfikatora roli
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");
    }
}
