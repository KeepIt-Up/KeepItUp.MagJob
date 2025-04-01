namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania DeleteOrganizationRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class DeleteOrganizationValidator : Validator<DeleteOrganizationRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="DeleteOrganizationValidator"/>.
    /// </summary>
    public DeleteOrganizationValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");
    }
}
