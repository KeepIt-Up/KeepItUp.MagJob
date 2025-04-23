namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Walidator dla żądania UpdateOrganizationRequest.
/// </summary>
/// <remarks>
/// Implementuje podstawową walidację techniczną danych wejściowych w warstwie Web.
/// </remarks>
public class UpdateOrganizationValidator : Validator<UpdateOrganizationRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationValidator"/>.
    /// </summary>
    public UpdateOrganizationValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        // Walidacja nazwy organizacji
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa organizacji jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa organizacji nie może przekraczać 100 znaków.");

        // Walidacja opisu organizacji (opcjonalny)
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis organizacji nie może przekraczać 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
