namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania CreateInvitationRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class CreateInvitationValidator : Validator<CreateInvitationRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateInvitationValidator"/>.
    /// </summary>
    public CreateInvitationValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        // Walidacja adresu email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Adres email jest wymagany.")
            .EmailAddress().WithMessage("Podany adres email jest nieprawidłowy.")
            .MaximumLength(100).WithMessage("Adres email nie może przekraczać 100 znaków.");

        // Walidacja identyfikatora roli
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Identyfikator roli jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator roli nie może być pusty (Guid.Empty).");
    }
}